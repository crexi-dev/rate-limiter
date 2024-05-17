using RateLimiter.Models;

namespace RateLimiter.Rules;
public interface IRuleCollection
{
    bool Validate(Request request);
}