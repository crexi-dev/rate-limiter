using RateLimiter.Rules;
using RateLimiter.Rules.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    public class RateLimiter : Dictionary<string, List<ILimitRule>>
    {
        private static readonly Lazy<RateLimiter> RateLimiterInstance = new(() => new RateLimiter());

        private RateLimiter()
        {
            LoadRules();
        }

        public static RateLimiter Instance => RateLimiterInstance.Value;

        public void Add(string ruleName, ILimitRule rule)
        {
            if (!this.ContainsKey(ruleName))
            {
                this[ruleName] = new List<ILimitRule>();
            }

            this[ruleName].Add(rule);
        }

        public bool IsRequestAllowed(string clientId, string ruleName)
        {
            return !this.ContainsKey(ruleName) ||
                   this[ruleName].All(rule => rule.IsRequestAllowed(clientId, ruleName));
        }

        private void LoadRules()
        {
            this.Add("fixedLimit", new FixedLimitRule(10, TimeSpan.FromMinutes(1)));
            this.Add("slidingLimit", new SlidingLimitRule(TimeSpan.FromSeconds(5)));
            this.Add(
                "regionLimit",
                new RegionBasedRule(
                    new FixedLimitRule(20, TimeSpan.FromMinutes(1)),
                    new SlidingLimitRule(TimeSpan.FromSeconds(5))));
        }
    }
}
