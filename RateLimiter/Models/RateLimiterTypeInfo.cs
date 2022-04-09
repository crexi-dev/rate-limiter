using RateLimiter.DataStorageSimulator;
using RateLimiter.Interfaces;
using System;

namespace RateLimiter.Models
{
    /// <summary>
    /// Model that contains Rate Limiter Info
    /// </summary>
    public class RateLimiterTypeInfo
    {
        public RateLimiterType Type { get; set; }
        public int RequestLimit { get; set; }
        public TimeSpan TimeInterval { get; set; }
        public IRateLimiterValidator? RateLimiterValidator { get; set; }
        public AvailableResource ResourceName { get; set; }

        public RateLimiterTypeInfo() 
        {
            ResourceName = AvailableResource.UnratedResource;
        }
    }
}
