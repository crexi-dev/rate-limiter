using System;

namespace RateLimiter.RateLimiter.Options
{
    public class TimespanLimiterOptions : ILimiterOptions
    {
        public int Limit => 1;
        public TimeSpan Window { get; set; }
    }
}
