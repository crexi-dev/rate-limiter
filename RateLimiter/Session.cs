namespace RateLimiter
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using static SessionState;
    using static SessionMessage;

    public sealed class Session : SessionBase, ISession, IDisposable
    {
        public string AccessToken { get; private set; }
        private RateLimit RateLimit;
        private readonly object SessionLocker = new object();
        private readonly object SessionValidationLocker = new object();

        /// <summary>
        /// Constructor to initilize in-memory session
        /// </summary>
        /// <param name="accessToken"></param>
        public Session(string accessToken)
        {
            UserAccessLimit = new ConcurrentDictionary<string, RateLimit>();
            AccessToken = accessToken;
            if (!string.IsNullOrEmpty(AccessToken))
                SetOrUpdateSession(AccessToken);
        }

        /// <summary>
        /// Method to check if the session is valid for a particular access-token
        /// </summary>
        /// <returns>ValidSession, InvalidToken or RateLimitExceeded</returns>
        public string IsValidSession()
        {
            if (string.IsNullOrEmpty(AccessToken)) return InvalidToken;
            SetOrUpdateSession(AccessToken);
            lock (SessionValidationLocker)
            {
                if (UserAccessLimit[AccessToken].CallCount >= SessionState.RateLimit)
                    return RateLimitExceeded;
                return ValidSession;
            }
        }

        /// <summary>
        /// Method to update in-memory session a particular access-token
        /// </summary>
        /// <param name="accessToken"></param>
        private protected override void SetOrUpdateSession(string accessToken)
        {
            lock (SessionLocker)
            {
                int CallCount = -2;
                RateLimit = UserAccessLimit.GetOrAdd(accessToken, RateLimit);
                if (RateLimit == null)
                {
                    Interlocked.Increment(ref CallCount);
                    RateLimit = new RateLimit { CallCount = CallCount };
                }
                else
                {
                    CallCount = RateLimit.CallCount;
                    Interlocked.Increment(ref CallCount);
                    if ((DateTime.Now.ToUniversalTime() - UserAccessLimit[AccessToken].FirstCall).TotalMilliseconds >= RateTimeLimit)
                    {
                        RateLimit.CallCount = -2;
                        RateLimit.FirstCall = DateTime.Now;
                    }
                    else
                    {
                        RateLimit.CallCount = CallCount;
                    }
                }
                UserAccessLimit.AddOrUpdate(accessToken, RateLimit, (at, count) => RateLimit);
            }
        }

        /// <summary>
        /// Dispose resources used to free up memory 
        /// </summary>
        public void Dispose()
        {
            AccessToken = null; RateLimit = null; UserAccessLimit = null;
        }
    }
}
