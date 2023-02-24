using System;
using System.Linq;
using RateLimiter.Configuration;

namespace RateLimiter
{
    internal class RateLimiter : IRateLimiter
    {
        private readonly RateLimiterOptions _options;
        private readonly AccessRegistry _registry;

        public RateLimiter(RateLimiterOptions options, AccessRegistry registry)
        {
            _options = options;
            _registry = registry;
        }

        public bool Check(string resource, string token)
        {
            _registry.AddAccessAttempt(resource, token, DateTime.UtcNow);

            var set = _options.GetRuleSet(resource);

            var rules = set?.GetRules();

            // empty rule set should pass
            if (!(rules?.Any() ?? false))
            {
                return true;
            }

            return rules.All(rule => rule.Check(_registry.GetAccessAttempts(resource, token)));
        }
    }
}