using System;

namespace RateLimiter;

public class RateLimitEntry
{
    public TimeSpan LastCall { get; set; }
    public int CallsCount { get; set; }
}