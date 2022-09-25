using System;

namespace RateLimiter.Models
{
    public class TimeSpanSinceLastCall
    {
        public DateTimeOffset LastCallTime { get; set; }
    }
}
