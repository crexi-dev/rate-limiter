using RateLimiter.Cache;

namespace RateLimiter.Rules
{
    public interface IRule
    {
        bool IsValid(UserRequest request, CacheEntry cacheEntry);
    }
}
