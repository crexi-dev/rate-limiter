using RateLimiter.Interfaces;
using System;

namespace RateLimiter.Implementation
{
    public class LimitByDateRangeRule : IRule
    {
        public int IntervalMS { get; set; }

        public int MaxReqeustCount { get; set; }

        public bool IsRuleValid(ICacheService cache, string token)
        {
            var key = $"{nameof(LimitByDateRangeRule)}_{token}";

            var value = cache.Get<RequestInfo>(key);

            if (value == null)
            {
                var info = new RequestInfo { Count = 1, FirstRequest = DateTime.UtcNow };
                cache.Set(key, info);
                return true;
            }

            if (value.FirstRequest.AddMilliseconds(IntervalMS) < DateTime.UtcNow)
            {
                var info = new RequestInfo { Count = 1, FirstRequest = DateTime.UtcNow };
                cache.Set(key, info);
                return true;
            }
            else if (value.Count < MaxReqeustCount)
            {
                var info = new RequestInfo { Count = ++value.Count, FirstRequest = value.FirstRequest };
                cache.Set(key, info);
                return true;
            }
            return false;
        }
    }
}
