using RateLimiter.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Decorators
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RateLimitRequest : Attribute
    {
        public RateLimiterType Type { get; set; }
        public int MaxRequests { get; set; }
        public int TimeWindowSeconds { get; set; }
        public string[] Regions { get; set; }

        public TimeSpan TimeWindow => TimeSpan.FromSeconds(TimeWindowSeconds);
    }
}
