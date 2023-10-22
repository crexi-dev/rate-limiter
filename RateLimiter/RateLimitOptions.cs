using System;

namespace RateLimiter;

public class RateLimitOptions
{
    public TimeSpan SinceLastCall { get; set; }
    public int CallsCountLimit { get; set; }
}