using System;
using System.Collections.Generic;
using RateLimiter.Library;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine.Library.Repository
{
    public class RuleRepository : IRuleRepository {
        private Dictionary<int, ResourceRule> resourceRules;
        private Dictionary<int, RegionRule> regionRules;
        private Dictionary<RateLimitLevel, Dictionary<RateLimitType, RateLimitSettingsBase>> rateLimitSettings;

        public RuleRepository() {
            // read rule data from db, file, etc.
            resourceRules = new Dictionary<int, ResourceRule>();
            regionRules = new Dictionary<int, RegionRule>();
            rateLimitSettings = new Dictionary<RateLimitLevel, Dictionary<RateLimitType, RateLimitSettingsBase>>();
            this.InitializeFakeRepository();
        }

        public ResourceRule GetResourceRule(int resourceId)
        {
            return this.resourceRules.ContainsKey(resourceId) ? this.resourceRules[resourceId] : null;
        }

        public RegionRule GetRegionRule(int id)
        {
            return this.regionRules.ContainsKey(id) ? this.regionRules[id] : null;
        }

        public void AddResourceRule(ResourceRule rule) {
            resourceRules.Add(this.regionRules.Count + 1, rule);
        }

        public void AddRegionRule(RegionRule rule) {            
            this.regionRules.Add(this.regionRules.Count + 1, rule);
        }

        public void UpdateResourceRule(ResourceRule rule)
        {
            this.resourceRules[rule.Id] = rule;
        }

        public void UpdateRegionRule(RegionRule rule)
        {
            this.regionRules[rule.Id] = rule;
        }

        private void InitializeFakeRepository() {
            // create rules
            var ruleUS = new RegionRule("US", Region.US, RateLimitType.RequestsPerTimespan, RateLimitLevel.Default);
            var ruleEU = new RegionRule("EU", Region.EU, RateLimitType.TimespanPassedSinceLastCall, RateLimitLevel.Default);

            this.AddRegionRule(ruleUS);
            this.AddRegionRule(ruleEU);
        }
    }
}