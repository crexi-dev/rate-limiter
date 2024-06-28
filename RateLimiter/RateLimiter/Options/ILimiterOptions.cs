using System;

namespace RateLimiter.RateLimiter.Options
{
    public interface ILimiterOptions
    {
        public int Limit { get; }
        public TimeSpan Window { get; }
    }
}
