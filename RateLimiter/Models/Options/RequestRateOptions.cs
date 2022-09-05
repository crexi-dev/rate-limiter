using System;

namespace RateLimiter.Models.Options
{
    public class RequestRateOptions
    {
        public const string Position = "RequestRate";

        public int MaxNumberOfRequests { get; set; } = 50;
        public TimeSpan RequestTimespanInMilliseconds { get; set; } = TimeSpan.FromMilliseconds(1000 * 60); // 1 min

    }
}
