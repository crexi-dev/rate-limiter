using System.Collections.Generic;

namespace RateLimiter
{
    public class LimiterConfiguration
    {
        public List<LimitRule> LimitRules { get; set; }
    }
}
