using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;


namespace RateLimiter.Limiter
{
    public class MixedResourceLimitter : IRequestLimitter
    {
        private List<IAllowRequest> appResourceRules = new List<IAllowRequest>();

        private Dictionary<Region, List<IAllowRequest>> appRules = new Dictionary<Region, List<IAllowRequest>>();
        private Dictionary<string, List<IAllowRequest>> rulesByName = new Dictionary<string, List<IAllowRequest>>();

        private IAllowRequest? requestRule;

        public void Configure(IAllowRequest rule)
        {
            requestRule = rule;
        }

        public MixedResourceLimitter(IEnumerable<IAllowRequest> rules)
        {
            appResourceRules.AddRange(rules);
        }

        public void AddRule(Region region, IEnumerable<IAllowRequest> rules)
        {
            appRules[region].AddRange(rules);
        }

        public void AddRule(string ruleName, IEnumerable<IAllowRequest> rules)
        {
            rulesByName[ruleName].AddRange(rules);
        }

        public bool Validate(string resourceValidate)
        {
            bool returnValue = false;

            if (appResourceRules.Count == 0 || string.IsNullOrEmpty(resourceValidate))
            {
                return returnValue;
            }

            appResourceRules.ForEach(rule =>
            {
               bool isAllowed = rule.IsResourceAllowed(resourceValidate);
                if(!isAllowed)
                {
                    returnValue = false;
                    return;
                }
                else
                {
                    returnValue = true;
                }
            });

            return returnValue;
        }

    }
}
