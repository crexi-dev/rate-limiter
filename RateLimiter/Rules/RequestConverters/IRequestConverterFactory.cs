using RateLimiter.Repositories;

namespace RateLimiter.Rules.RequestConverters;

public interface IRequestConverterFactory
{
    IRequestConverter Create(RuleValue value);
}
