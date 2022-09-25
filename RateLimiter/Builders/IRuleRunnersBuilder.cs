using RateLimiter.Models;
using RateLimiter.RuleRunners;
using System.Collections.Generic;

namespace RateLimiter.Builders
{
    public interface IRuleRunnersBuilder
    {
        IEnumerable<IRuleRunner> GetRuleRunners(RateLimitRuleOptions options, ClientRequest request);
    }
}
