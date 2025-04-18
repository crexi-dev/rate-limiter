using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Core.Rules
{
    public class RegionBasedRule : IRateLimitRule
    {
        private readonly Dictionary<string, IRateLimitRule> _regionRules;
        private readonly Func<string, string> _regionExtractor;
        private readonly IRateLimitRule _defaultRule;

        public RegionBasedRule(Dictionary<string, IRateLimitRule> regionRules, Func<string, string> regionExtractor, IRateLimitRule defaultRule = null)
        {
            _regionRules = regionRules;
            _regionExtractor = regionExtractor;
            _defaultRule = defaultRule;
        }

        public bool IsAllowed(string clientToken, string resourceId)
        {
            // get region
            var region = _regionExtractor(clientToken);

            // get the rule for this region else use default
            if (_regionRules.TryGetValue(region, out var rule))
            {
                bool result = rule.IsAllowed(clientToken, resourceId);
                return result;
            }

            // if no rule for region and no default, allow request
            if (_defaultRule != null)
            {
                return _defaultRule.IsAllowed(clientToken, resourceId);
            }

            return true;
        }
    }
}