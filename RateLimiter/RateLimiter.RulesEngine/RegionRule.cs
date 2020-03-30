using System;

namespace RateLimiter.RulesEngine
{
    public class RegionRule : Rule {
        public RegionRule(int id, string name, RateLimitType rateLimitType, RateLimitLevel rateLimitLevel)
            : base(id, name, rateLimitType, rateLimitLevel) {
        }
    }
}