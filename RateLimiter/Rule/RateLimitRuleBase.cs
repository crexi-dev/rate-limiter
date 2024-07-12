using System;
using System.Collections.Generic;
using RateLimiter.Model;

namespace RateLimiter.Rule
{
    public abstract class RateLimitRuleBase
    {
        public abstract bool CheckRequestAllow(IEnumerable<RateLimitRequest> requestData);
    }
}
