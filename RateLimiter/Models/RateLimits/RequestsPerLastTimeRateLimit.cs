using System;
using RateLimiter.Abstractions;

namespace RateLimiter.Models.RateLimits;

public class RequestsPerLastTimeRateLimit : IRateLimit
{
    public TimeSpan TimeSpan { get; set; }
    public long MaxCallCount { get; set; }
}