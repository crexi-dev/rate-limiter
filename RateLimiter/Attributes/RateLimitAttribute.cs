using System;

namespace RateLimiter.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public abstract class RateLimitAttribute : Attribute
{
    public int TimeWindowInSeconds { get; set; }
    public int MaxRequests { get; set; }
}