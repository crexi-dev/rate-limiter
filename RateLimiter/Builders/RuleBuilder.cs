using System;
using System.Collections.Generic;
using RateLimiter.Core.Rules;

namespace RateLimiter.Builders
{
    public static class RuleBuilder
    {
        public static IRateLimitRule CreateFixedWindowRule(int maxRequests, TimeSpan window)
        {
            return new FixedWindowRule(maxRequests, window);
        }

        public static IRateLimitRule CreateTokenBucketRule(int capacity, double refillRate)
        {
            return new TokenBucketRule(capacity, refillRate);
        }

        public static IRateLimitRule CreateRegionBasedRule(Dictionary<string, IRateLimitRule> regionRules, Func<string, string> regionExtractor, IRateLimitRule? defaultRule = null)
        {
            return new RegionBasedRule(regionRules, regionExtractor, defaultRule);
        }

        public static IRateLimitRule CreateCompositeAndRule(params IRateLimitRule[] rules)
        {
            return new CompositeAndRule(rules);
        }

        public static IRateLimitRule CreateCompositeOrRule(params IRateLimitRule[] rules)
        {
            return new CompositeOrRule(rules);
        }

        public static IRateLimitRule CreateRegionRules()
        {
            // set region rules
            var usRule = new FixedWindowRule(5, TimeSpan.FromMinutes(1));
            var euRule = new TokenBucketRule(3, 3/60.0);

            // create region mapping
            var regionRules = new Dictionary<string, IRateLimitRule>
            {
                { "US", usRule },
                { "EU", euRule }
            };

            // create region rules
            return CreateRegionBasedRule(regionRules, GetRegion);
        }

        private static string GetRegion(string token)
        {
            if (token.StartsWith("EU"))
            {
                return "EU";
            }
            return "US";
        }
    }
}