using System;
using System.Collections.Generic;

namespace RateLimiter.RulesEngine.Library.Rules
{
    public class ResourceRule : Rule
    {
        public ResourceRule(int id, string name, RateLimitType rateLimitType, RateLimitLevel rateLimitLevel)
            : base(id, name, rateLimitType, rateLimitLevel)
        {
        }
    }
}