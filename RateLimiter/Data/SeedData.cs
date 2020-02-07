using RateLimiter.Models;
using System;

namespace RateLimiter.Data
{
    public static class SeedData
    {
        public static FrequencyLimitRule FrequencyLimitRule = new FrequencyLimitRule
        {
            NumberOfRequests = 2,
            TimeSpanLimit = new TimeSpan(0, 0, 0, 5)
        };

        public static TimeSpanLimitRule TimeSpanLimitRule = new TimeSpanLimitRule
        {
            TimeSpanLimit = new TimeSpan(0, 0, 0, 3)
        };

        public static Guid Token1 = Guid.NewGuid();
        public static Guid Token2 = Guid.NewGuid();
        public static Guid Token3 = Guid.NewGuid();
    }
}
