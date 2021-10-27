using System.Collections.Generic;

namespace RateLimiter.Domain.ApiLimiter
{
    public interface ITokenBucket
    {
        void AddRules(IEnumerable<IRule> rules);
        bool Verify(string token);
    }
}