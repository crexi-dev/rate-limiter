using System.Collections.Generic;

namespace RateLimiter.Models
{
    public class RateLimitOptions
    {
        public List<ILimitRule> LimitRules { get; set; }
    }
}
