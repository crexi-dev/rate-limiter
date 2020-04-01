using RateLimiter.Library;
using RateLimiter.RulesEngine.Library;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine
{
    public class RulesEngine : IRulesEngine {
        private IRuleRepository ruleRepository;
        private IRulesEvaluator rulesEvaluator;

        public RulesEngine(IRuleRepository ruleRepository, IRulesEvaluator rulesEvaluator) {
            this.ruleRepository = ruleRepository;
            this.rulesEvaluator = rulesEvaluator;
        }

        public void AddRegionRule(RegionRule rule) {
            // if rule type is Region, translate region to 

            this.ruleRepository.AddRegionRule(rule);
        }

        public void AddResourceRule(ResourceRule rule)
        {
            // if rule type is Region, translate region to 

            this.ruleRepository.AddResourceRule(rule);
        }

        public RateLimitSettingsConfig Evaluate(string resource, string IPAddress)
        {
            var resourceRule = ruleRepository.GetResourceRule(resource);
            RateLimitSettingsConfig rateLimitSettingsConfig = new RateLimitSettingsConfig();

            if (resourceRule != null)
            {
                switch(resourceRule.RateLimitLevel)
                {
                    case RateLimitLevel.Default:
                        rateLimitSettingsConfig[RateLimitType.RequestsPerTimespan] = new RequestsPerTimespanSettings()
                        {
                            MaxAmount = 5,
                            RefillAmount = 5,
                            RefillTime = 60

                        };

                    break;
                }

                return rateLimitSettingsConfig;
            }

            return null;
        }
    }
}