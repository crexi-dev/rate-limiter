using System;

namespace RateLimiter.Models.Options
{
    public class RequestRateOptions
    {
        public int MaxNumberOfRequests { get; set; } // 50;
        public int RequestTimespanInMilliseconds { get; set; } // 1000 * 60; // 1 min

    }
}
