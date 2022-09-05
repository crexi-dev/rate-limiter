using System;

namespace RateLimiter.Models.Options
{
    public class LastCallTimeSpanOptions
    {
        public const string Position = "LastCallTimeSpan";

        public TimeSpan MinRequestTimespanInMilliseconds { get; set; } = TimeSpan.FromMilliseconds(250);
    }
}
