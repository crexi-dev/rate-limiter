using System.Collections;

namespace RateLimiter.Rules;
public interface IRuleCollection : ICollection
{
    bool ValidateRules(Request token);
}