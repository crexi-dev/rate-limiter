using System.Collections;

namespace RateLimiter.Rules;
public interface IRuleCollection
{
    bool Validate(Request request);
}