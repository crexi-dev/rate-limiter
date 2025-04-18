using System;
using RateLimiter.Core;
using RateLimiter.Core.Rules;
using RateLimiter.Builders;

namespace RateLimiter.Examples
{
    public static class RateLimiterExamples
    {
        public static Core.RateLimiter CreateExampleRateLimiter()
        {
            var rateLimiter = new Core.RateLimiter();

            // example configurations
            rateLimiter.ConfigureResource("/api/test", RuleBuilder.CreateTokenBucketRule(3, 3/60.0));
            rateLimiter.ConfigureResource("/api/orders", RuleBuilder.CreateFixedWindowRule(10, TimeSpan.FromMinutes(1)));
            rateLimiter.ConfigureResource("/api/orders/item", RuleBuilder.CreateRegionRules());

            // composite rule examples
            var rule1 = new TokenBucketRule(1000, 1000/3600.0);
            var rule2 = new FixedWindowRule(5, TimeSpan.FromSeconds(10));
            rateLimiter.ConfigureResource("/api/users", new CompositeAndRule(rule1, rule2));

            return rateLimiter;
        }
    }
}