using RateLimiter.Contracts;
using System.Collections.Generic;

namespace RateLimiter.Models
{
    public class RateLimiterSettings : IRateLimiterSettings
    {
        public IEnumerable<IRequestLimitRule> Rules { get; set; }
    }
}
