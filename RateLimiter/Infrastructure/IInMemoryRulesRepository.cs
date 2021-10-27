using RateLimiter.Domain.Resource;

namespace RateLimiter.Infrastructure
{
    public interface IInMemoryRulesRepository
    {
        ResourceRules ResourceRules { get; }
    }
}