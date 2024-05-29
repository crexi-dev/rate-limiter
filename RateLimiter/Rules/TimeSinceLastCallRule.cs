using System;
using System.Collections.Generic;
using RateLimiter.Interfaces;

namespace RateLimiter.Rules
{

    public class TimeSinceLastCallRule : IRule
    {
        private readonly TimeSpan _timeSpan;
        private readonly Dictionary<string, DateTime> _tokenLastRequestTimes;

        public TimeSinceLastCallRule(TimeSpan timeSpan)
        {
            _timeSpan = timeSpan;
            _tokenLastRequestTimes = new Dictionary<string, DateTime>();
        }

        public bool IsAllowed(string token)
        {
            lock (_tokenLastRequestTimes)
            {
                if (!_tokenLastRequestTimes.TryGetValue(token, out var lastRequestTime))
                {
                    lastRequestTime = DateTime.MinValue;
                }

                var now = DateTime.UtcNow;
                if (now - lastRequestTime > _timeSpan)
                {
                    _tokenLastRequestTimes[token] = now;
                    return true;
                }

                return false;
            }
        }
    }
}
