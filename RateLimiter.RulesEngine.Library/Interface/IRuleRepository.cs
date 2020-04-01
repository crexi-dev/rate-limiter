using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine.Library
{
    public interface IRuleRepository {
        void AddResourceRule(ResourceRule rule);
        void AddRegionRule(RegionRule rule);
        void UpdateResourceRule(ResourceRule rule);
        void UpdateRegionRule(RegionRule rule);
        ResourceRule GetResourceRule(int id);
        RegionRule GetRegionRule(int id);
    }
}