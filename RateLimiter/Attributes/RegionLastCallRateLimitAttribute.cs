using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Attributes.Interfaces;

namespace RateLimiter.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RegionLastCallRateLimitAttribute : Attribute, IRateLimiterAttribute
{
    public TimeSpan TimeSpan { get; }
    public List<string> Regions { get; }
    
    public RegionLastCallRateLimitAttribute(int seconds, params string[] regions)
    {
        TimeSpan = TimeSpan.FromSeconds(seconds);
        Regions = regions.ToList();
    }
}