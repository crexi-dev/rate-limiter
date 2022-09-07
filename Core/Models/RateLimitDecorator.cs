using Core.Common;
using System;

namespace Core.Models
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RateLimitDecorator : Attribute
    {
        public RateLimitStrategyEnum[] StrategyTypes { get; set; }
        public int TimeSpan { get; set; }
        public int MaxRequests { get; set; }
    }
}
