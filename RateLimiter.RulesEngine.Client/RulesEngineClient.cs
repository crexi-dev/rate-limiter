using RateLimiter.Library;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine.Client
{
    public class RulesEngineClient : IRulesEngine {
        private IRulesEngineProxy rulesEngineProxy;

        public RulesEngineClient(IRulesEngineProxy rulesEngineProxy) {
            this.rulesEngineProxy = rulesEngineProxy;
        }

        public void AddResourceRule(ResourceRule rule)
        {
        }

        public void AddRegionRule(RegionRule rule)
        {
        }

        public RateLimitSettingsConfig Evaluate(string resource, string IPAddress)
        {
            return new RateLimitSettingsConfig();
        }
    }
}