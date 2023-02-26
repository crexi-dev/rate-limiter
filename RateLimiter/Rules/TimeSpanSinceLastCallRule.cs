using System;
using System.Collections.Generic;

namespace RateLimiter.Rules
{
    public class TimeSpanSinceLastCallRule : IRule
    {
        private readonly TimeSpan _timeSpan;
        private readonly Dictionary<string, DateTime> _lastCallTimes = new Dictionary<string, DateTime>();

        public TimeSpanSinceLastCallRule(TimeSpan timeSpan)
        {
            _timeSpan = timeSpan;
        }

        public bool IsRequestAllowed(string resource, string client)
        {
            var key = $"{resource}:{client}";
            if (_lastCallTimes.TryGetValue(key, out var lastCallTime))
            {
                if ((DateTime.UtcNow - lastCallTime) < _timeSpan)
                {
                    return false;
                }
            }
            _lastCallTimes[key] = DateTime.UtcNow;
            return true;
        }
    }
}