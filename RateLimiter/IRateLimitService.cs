using System;
using System.Collections.Generic;
using RateLimiter.Model;
using RateLimiter.Rule;

namespace RateLimiter
{
    public interface IRateLimitService
    {
        void AddRule<T>(Uri uri, T rule) where T : RateLimitRuleBase;
        public IEnumerable<RateLimitRuleBase> GetRules(Uri uri);
        public void ClearRules(Uri uri);
        bool RequestAllow(RateLimitRequest request);
    }
}
