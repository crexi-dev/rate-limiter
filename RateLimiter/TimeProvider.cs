using System;
using RateLimiter.Interfaces;

namespace RateLimiter;

public class TimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.Now;
}