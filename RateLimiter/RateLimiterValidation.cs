using RateLimiter.Rules;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    public class RateLimiterValidation
    {
        public bool IsRequestValid(IEnumerable<IRateLimiterRule> rateLimiterRules)
        {
            return rateLimiterRules.All(x => x.IsValid());
        }
    }
}
