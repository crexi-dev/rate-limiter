using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine.Library
{
    public interface IRuleRepository {
        void AddResourceRule(ResourceRule rule);
        void AddRegionRule(RegionRule rule);
        ResourceRule GetResourceRule(string resource);
        RegionRule GetRegionRule(Region region);
        ResourceRegionRule GetResourceRegionRule(string resource, Region region);
    }
}