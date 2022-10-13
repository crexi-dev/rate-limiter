using System;

namespace RateLimiter
{
    /// <summary>
    /// Defines how many requests can pass through within defined time span
    /// </summary>
    internal class RequestsPerTimespanLimitStrategyConfiguration
    {
        public int MaxAllowedRequests { get; set; }

        public TimeSpan TimeSpan { get; set; }  
    }
}
