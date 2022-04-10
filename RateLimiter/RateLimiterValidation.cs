using RateLimiter.Rules;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    public interface IRateLimiterValidation
    {
        bool IsRequestValid(IEnumerable<IRateLimiterRule> rateLimiterRules);
    }

    public class RateLimiterValidation : IRateLimiterValidation
    {
        public bool IsRequestValid(IEnumerable<IRateLimiterRule> rateLimiterRules)
        {
            return rateLimiterRules.All(x => x.IsValid());
        }
    }
}
