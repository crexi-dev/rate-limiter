using System;

namespace RateLimiter.Configuration.Options
{
    public class LocationLimiterOptions
    {
        public string? LocationName { get; set; }

        public TimeSpan TimeRange { get; set; }

        public int AllowedRequestsCountPerTimeRange { get; set; }
    }
}