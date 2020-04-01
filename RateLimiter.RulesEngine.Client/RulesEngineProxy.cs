using RateLimiter.Library;
using RateLimiter.RulesEngine.Library.Repository;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine.Client
{
    public class RulesEngineProxy : IRulesEngine {
        private RulesEngine rulesEngine;

        public RulesEngineProxy()
        {
            var rulesRepository = new RuleRepository();
        }

        public void AddResourceRule(ResourceRule rule)
        {
        }

        public void AddRegionRule(RegionRule rule)
        {
        }

        public void UpdateResourceRule(ResourceRule rule)
        {

        }

        public void UpdateRegionRule(RegionRule rule)
        {

        }


        public RateLimitSettingsConfig Evaluate(string resource, string IPAddress)
        {
            return new RateLimitSettingsConfig();
        }
    }
}