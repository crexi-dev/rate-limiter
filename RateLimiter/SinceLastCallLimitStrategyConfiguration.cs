using System;

namespace RateLimiter
{
    internal class SinceLastCallLimitStrategyConfiguration
    {
        public TimeSpan TimeSpan { get; set; }
    }
}
