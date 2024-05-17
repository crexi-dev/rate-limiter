using RateLimiter.Models;

namespace RateLimiter.Rules;

public abstract class Rule{
    public abstract bool Validate(RuleRequestInfo token);
}
