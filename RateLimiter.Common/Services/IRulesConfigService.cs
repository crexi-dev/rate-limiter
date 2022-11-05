using RateLimiter.RateLimitRules;
using System.Collections.Generic;

namespace RateLimiter
{
    public interface IRulesConfigService
    {
        void SetRules(string key, List<IRateLimitRule> rules);
    }
}
