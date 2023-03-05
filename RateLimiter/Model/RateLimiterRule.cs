using System;

namespace RateLimiter.Model
{
    public class RateLimiterRule
    {
        public RateLimiterRule(TimeSpan period, int limit)
        {
            Period = period;
            Limit = limit;
        }

        public TimeSpan Period { get; private set; }

        public int Limit { get; private set; }

        public static RateLimiterRule Create(TimeSpan period, int limit) => new RateLimiterRule(period, limit);
    }
}
