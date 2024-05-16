using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;

namespace RateLimiter.Limiter
{
    public enum Region
    {
        US,
        EU
    }

    public class ResourceLimitter : IRequestLimitter
    {
        private IAllowRequest ?requestRule;
        private Dictionary<Region, IAllowRequest> rules = new Dictionary<Region, IAllowRequest>();
        private Dictionary<string, IAllowRequest> rulesByName = new Dictionary<string, IAllowRequest>();

        public void Configure(IAllowRequest rule)
        {
            requestRule = rule;
        }

        public void AddRule(Region region, IAllowRequest rule)
        {
            rules[region] = rule;
        }

        public void AddRule(string ruleName, IAllowRequest rule)
        {
            rulesByName[ruleName] = rule;
        }

        public bool IsAllowedByRegion(Region region, string resource)
        {
            if (!rules.ContainsKey(region))
            {
                return false;     
            }

            return rules[region].IsResourceAllowed(resource);
        }

        public bool IsAllowed(string rule, string resource)
        {
            if (!rulesByName.ContainsKey(rule))
            {
                return false;
            }

            return rulesByName[rule].IsResourceAllowed(resource);
        }

        public bool Validate(string resourceValidate)
        {
            if(requestRule == null || string.IsNullOrEmpty(resourceValidate))
            {
                return false;
            }

            return requestRule.IsResourceAllowed(resourceValidate);
        }
    }
}