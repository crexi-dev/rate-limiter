using System.Collections.Generic;

namespace RateLimiter.Rules
{
    public interface IRateLimitRuleFactory
    {
        List<IRateLimitRule> GetRateLimitRulesByResource(string resourceName);
    }
}
