using RuleLimiterTask.Cache;

namespace RuleLimiterTask.Rules
{
    public interface IRule
    {
        bool IsValid(UserRequest request, CacheEntry cacheEntry);
    }
}
