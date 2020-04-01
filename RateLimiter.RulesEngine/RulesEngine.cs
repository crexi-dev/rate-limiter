using System;
using System.Collections.Generic;
using RateLimiter.Library;
using RateLimiter.RulesEngine.Library;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine
{
    public class RulesEngine : IRulesEngine {
        private Dictionary<string, ResourceRule> activeRegionRules;
        private IRuleRepository ruleRepository;
        private IRuleCache ruleCache;
        private IRulesEvaluator rulesEvaluator;

        public RulesEngine(IRuleRepository ruleRepository, IRuleCache ruleCache, IRulesEvaluator rulesEvaluator) {
            this.ruleCache = ruleCache;
            this.ruleRepository = ruleRepository;
            this.rulesEvaluator = rulesEvaluator;
        }

        public void AddRegionRule(RegionRule rule) {
            this.ruleRepository.AddRegionRule(rule);
        }

        public void AddResourceRule(ResourceRule rule)
        {
            // if rule type is Region, translate region to 

            this.ruleRepository.AddResourceRule(rule);
        }

        public void UpdateResourceRule(ResourceRule rule)
        {

        }

        public void UpdateRegionRule(RegionRule rule)
        {

        }

        public RateLimitSettingsConfig Evaluate(string resource, string IPAddress)
        {
            Region region = GetRegionFromIPGeoLocationService(IPAddress);

            // lookup rule in cache in order of rule specificity (most specific to least specific)
            Rule rule;

            // 1. resource + region rule
            rule = (ResourceRule) this.ruleCache[resource + region.ToString()];

            // 2. global resource rule
            if (rule == null)
                rule = (ResourceRule) this.ruleCache[resource + Region.All.ToString()];
            
            else
                // 3. region rule
                rule = (RegionRule) this.ruleCache[region.ToString()];

            // if no rules apply, return default settings
            if (rule == null)
                return new DefaultRateLimiterSettings().RateLimiterSettings;

            else
            {
                // populate rate limit settings
                RateLimitSettingsConfig rateLimitSettingsConfig = new RateLimitSettingsConfig();

                switch (rule.RateLimitLevel)
                {
                    case RateLimitLevel.Low:
                        rateLimitSettingsConfig[RateLimitType.RequestsPerTimespan] = new TokenBucketSettings()
                        {
                            MaxAmount = 1,
                            RefillAmount = 1,
                            RefillTime = 60

                        };

                        rateLimitSettingsConfig[RateLimitType.TimespanPassedSinceLastCall] = new TimespanPassedSinceLastCallSettings()
                        {
                            TimespanLimit = new TimeSpan(0, 30, 0)
                        };

                        break;

                    case RateLimitLevel.Default:
                        rateLimitSettingsConfig[RateLimitType.RequestsPerTimespan] = new TokenBucketSettings()
                        {
                            MaxAmount = 5,
                            RefillAmount = 5,
                            RefillTime = 60

                        };

                    break;
                }

                return rateLimitSettingsConfig;
            }
        }

        private Region GetRegionFromIPGeoLocationService(string IPAddress)
        {
            return Region.US;
        }
    }
}