using System;

namespace RateLimiter.Models
{
    public class Rule
    {
        public int RequestsMaxCount { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}
