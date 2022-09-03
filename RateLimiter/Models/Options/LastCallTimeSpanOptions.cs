using System;

namespace RateLimiter.Models.Options
{
    public class LastCallTimeSpanOptions
    {
        public TimeSpan MinRequestTimespan { get; set; } = TimeSpan.FromMilliseconds(250);
    }
}
