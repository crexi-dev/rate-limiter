using System;
using RateLimiter.Abstractions;

namespace RateLimiter.Models.RateLimits;

public class TimeFromLastCallRateLimit : IRateLimit
{
    public TimeSpan TimeFromLastCall { get; set; }
}