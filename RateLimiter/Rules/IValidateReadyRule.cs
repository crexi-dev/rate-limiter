using RateLimiter.Rules.RequestConverters;

namespace RateLimiter.Rules;

public interface IValidateReadyRule
{
    IRule Rule { get; }
    IRequestConverter RequestConverter { get; }
}