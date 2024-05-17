using RateLimiter.Models;

namespace RateLimiter.Rules;

public interface IRule
{
    bool Validate(Request request);
}
