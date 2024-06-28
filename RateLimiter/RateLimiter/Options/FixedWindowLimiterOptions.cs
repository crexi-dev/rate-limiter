using System;

namespace RateLimiter.RateLimiter.Options
{
    public class FixedWindowLimiterOptions : ILimiterOptions
    {
        public int Limit { get; set; }
        public TimeSpan Window { get; set; }
    }
}
