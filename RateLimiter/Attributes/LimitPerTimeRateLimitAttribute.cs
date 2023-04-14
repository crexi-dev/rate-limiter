using System;
using RateLimiter.Attributes.Interfaces;

namespace RateLimiter.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class LimitPerTimeRateLimitAttribute : Attribute, IRateLimiterAttribute
{
    public int Limit { get; }
    public TimeSpan TimeSpan { get; }
    
    public LimitPerTimeRateLimitAttribute(int limit, int seconds)
    {
        Limit = limit;
        TimeSpan = TimeSpan.FromSeconds(seconds);
    }
}