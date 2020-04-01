using RateLimiter.Library;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine
{
    public interface IRulesEngine {
        void AddResourceRule(ResourceRule rule);
        void AddRegionRule(RegionRule rule);
        void UpdateResourceRule(ResourceRule rule);
        void UpdateRegionRule(RegionRule rule);
        RateLimitSettingsConfig Evaluate(string resource, string IPAddress);
    }
}