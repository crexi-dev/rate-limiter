using RateLimiter.Models;

namespace RateLimiter.Rules;

public interface IRule
{
    bool Validate(RuleRequestInfo? requestInfo);
}
