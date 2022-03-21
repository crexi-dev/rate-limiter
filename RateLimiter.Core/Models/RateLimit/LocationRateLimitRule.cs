using System.Collections.Generic;
using RateLimiter.Core.Models.Enums;

namespace RateLimiter.Core.Models.RateLimit
{
    public class LocationRateLimitRule
    {
        public List<RateLimitRule> Rules { get; set; } = new();

        public LocationEnum Location { get; set; }
    }
}