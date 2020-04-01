using System;
using System.Collections.Generic;
using RateLimiter.Library;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine.Library.Repository
{
    public class RuleRepository : IRuleRepository {
        private Dictionary<string, ResourceRule> resourceRules;
        private Dictionary<Region, RegionRule> regionRules;
        private Dictionary<Tuple<string, Region>, ResourceRegionRule> resourceRegionRules;
        private Dictionary<RateLimitLevel, Dictionary<RateLimitType, RateLimitSettingsBase>> rateLimitSettings;

        public RuleRepository() {
            // read rule data from db, file, etc.
            resourceRules = new Dictionary<string, ResourceRule>();
            regionRules = new Dictionary<Region, RegionRule>();
            resourceRegionRules = new Dictionary<Tuple<string, Region>, ResourceRegionRule>();
            rateLimitSettings = new Dictionary<RateLimitLevel, Dictionary<RateLimitType, RateLimitSettingsBase>>();
            this.InitializeFakeRepository();
        }

        public ResourceRule GetResourceRule(string resource)
        {
            return this.resourceRules.ContainsKey(resource) ? this.resourceRules[resource] : null;
        }

        public RegionRule GetRegionRule(Region region)
        {
            return this.regionRules.ContainsKey(region) ? this.regionRules[region] : null;
        }

        public ResourceRegionRule GetResourceRegionRule(string resource, Region region)
        {
            var key = new Tuple<string, Region>(resource, region);
            return this.resourceRegionRules.ContainsKey(key)
                ? this.resourceRegionRules[key]
                : null;
        }

        public void AddResourceRule(ResourceRule rule) {
            resourceRules.Add(rule.ResourceName, rule);
        }

        public void AddRegionRule(RegionRule rule)
        {
            regionRules.Add(rule.Region, rule);
        }

        private void InitializeFakeRepository() {
            // create rules
            var ruleUS = new RegionRule("US", Region.US, RateLimitType.RequestsPerTimespan, RateLimitLevel.Default);
            var ruleEU = new RegionRule("EU", Region.EU, RateLimitType.TimespanPassedSinceLastCall, RateLimitLevel.Default);
            var ruleCombo = new ResourceRegionRule("Combo rule", "/api/resource1", Region.US, RateLimitType.RequestsPerTimespan | RateLimitType.TimespanPassedSinceLastCall, RateLimitLevel.Default);

            regionRules[Region.US] = ruleUS;
            regionRules[Region.EU] = ruleEU;
            var resourceRegionKey = new Tuple<string, Region>("/api/resource1", Region.US);
            resourceRegionRules[resourceRegionKey] = ruleCombo;
        }
    }
}