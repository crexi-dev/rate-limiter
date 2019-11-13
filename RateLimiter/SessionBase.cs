namespace RateLimiter
{
    using System.Collections.Concurrent;

    public abstract class SessionBase
    {
        private protected static ConcurrentDictionary<string, RateLimit> UserAccessLimit { get; set; }
        private protected abstract void SetOrUpdateSession(string accessToken);
    }
}
