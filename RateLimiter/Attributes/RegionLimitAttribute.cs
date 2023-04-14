using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Attributes.Interfaces;

namespace RateLimiter.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RegionLimitAttribute : Attribute, IRateLimiterAttribute
{
    public List<string> Regions { get; }
    
    public RegionLimitAttribute(params string[] regions)
    {
        Regions = regions.ToList();
    }
    
    public RegionLimitAttribute(List<string> regions)
    {
        Regions = regions;
    }
}