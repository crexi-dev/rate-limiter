using System;
using RateLimiter.Attributes.Interfaces;

namespace RateLimiter.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class LastCallRateLimitAttribute : Attribute, IRateLimiterAttribute
{
    public TimeSpan TimeDelta { get; set; }
    
    public LastCallRateLimitAttribute(int seconds)
    {
        TimeDelta = TimeSpan.FromSeconds(seconds);
    }
    
    public LastCallRateLimitAttribute(TimeSpan timeDelta)
    {
        TimeDelta = timeDelta;
    }
}