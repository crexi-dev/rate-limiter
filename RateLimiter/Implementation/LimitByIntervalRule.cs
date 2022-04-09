using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Implementation
{
    public class LimitByIntervalRule : IRule
    {
        public int IntervalMS { get; set; }

        public bool IsRuleValid(ICacheService cache, string token)
        {
            var key = $"{nameof(LimitByIntervalRule)}_{token}";

            var value = cache.Get<RequestInfo>(key);

            if (value == null)
            {
                var info = new RequestInfo { LastRequest = DateTime.UtcNow };
                cache.Set(key, info);
                return true;
            }

            if (value.LastRequest.AddMilliseconds(IntervalMS) < DateTime.UtcNow)
            {
                var info = new RequestInfo { LastRequest = DateTime.UtcNow };
                cache.Set(key, info);
                return true;
            }
            return false;
        }
    }
}
