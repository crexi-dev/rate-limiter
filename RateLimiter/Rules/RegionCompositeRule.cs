using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RateLimiter.Abstractions;
using RateLimiter.Models;

namespace RateLimiter.Rules
{
    public class RegionCompositeRule : IRateLimitRule
    {
        private readonly Dictionary<string, IRateLimitRule> _regionRules;
        private readonly IRateLimitRule _defaultRule;
        
        public RegionCompositeRule(IRateLimitRule defaultRule)
        {
            _defaultRule = defaultRule ?? throw new ArgumentNullException(nameof(defaultRule));
            _regionRules = new Dictionary<string, IRateLimitRule>(StringComparer.OrdinalIgnoreCase);
        }
        
        public void AddRegionRule(string region, IRateLimitRule rule)
        {
            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentException("Region cannot be null or empty", nameof(region));
            }
            
            _regionRules[region] = rule ?? throw new ArgumentNullException(nameof(rule));
        }
        
        public Task<bool> ValidateAsync(RequestContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            
            var rule = GetRuleForRegion(context.Region);
            
            return rule.ValidateAsync(context);
        }
        
        private IRateLimitRule GetRuleForRegion(string region)
        {
            if (!string.IsNullOrWhiteSpace(region) && _regionRules.TryGetValue(region, out var rule))
            {
                return rule;
            }
            
            return _defaultRule;
        }
    }
}