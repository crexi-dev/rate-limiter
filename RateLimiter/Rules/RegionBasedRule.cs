using RateLimiter.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Rules
{
    public class RegionBasedRule : IRateLimiterRule
    {
        private readonly IApiClient apiClient;
        private readonly IRegionBasedRuleFactory ruleFactory;

        public RegionBasedRule(IApiClient apiClient,
                               IRegionBasedRuleFactory ruleFactory)
        {
            this.apiClient = apiClient;
            this.ruleFactory = ruleFactory;
        }

        public bool Validate()
        {
            var rule = ruleFactory.GetRateLimiterRule(apiClient.Region);
            return rule.Validate();
        }
    }
}
