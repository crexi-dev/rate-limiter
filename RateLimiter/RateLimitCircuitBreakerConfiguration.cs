using System;

namespace RateLimiter
{
    internal class RateLimitCircuitBreakerConfiguration
    {
        public TimeSpan LockFor { get; set; }
    }
}
