using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Domain.Aggregate
{
    public class Resource
    {
        public string Key { get; set; }

        public IEnumerable<LimitRule> RateLimitRules { get; set; }

        public bool HasNoLimitation() => RateLimitRules == null || !RateLimitRules.Any();
    }
}
