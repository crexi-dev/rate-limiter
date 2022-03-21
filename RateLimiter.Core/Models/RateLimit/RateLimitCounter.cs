using System;

namespace RateLimiter.Core.Models.RateLimit
{
    public class RateLimitCounter
    {
        public DateTime Timestamp { get; set; }

        public double Count { get; set; }
    }
}