using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Classes.Rules
{
    public class TimeSpanSinceLastCallRule : IRateLimitRule
    {
        private readonly TimeSpan _minTimeSpanBetweenCalls;

        public TimeSpanSinceLastCallRule(TimeSpan minTimeSpanBetweenCalls)
        {
            _minTimeSpanBetweenCalls = minTimeSpanBetweenCalls;
        }

        public bool IsRequestAllowed(string token, string resource)
        {
            var key = $"{resource}:{token}";
            if (!MemoryStore.Requests.ContainsKey(key) || !MemoryStore.Requests[key].Any())
            {
                MemoryStore.Requests[key] = new List<DateTime> { DateTime.Now };
                return true;
            }

            var lastRequestTime = MemoryStore.Requests[key].Last();
            if (DateTime.Now - lastRequestTime >= _minTimeSpanBetweenCalls)
            {
                MemoryStore.Requests[key].Add(DateTime.Now);
                return true;
            }

            return false;
        }
    }
}
