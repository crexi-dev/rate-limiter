using System;

namespace RateLimiter.Models.Options
{
    public class RequestRateOptions
    {
        public const string Position = "RequestRate";

        public int MaxNumberOfRequests { get; set; } = 50;
        public TimeSpan RequestTimespan { get; set; } = TimeSpan.FromMinutes(1);

    }
}
