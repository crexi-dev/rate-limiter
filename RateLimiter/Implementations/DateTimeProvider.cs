using System;
using RateLimiter.Interfaces;

namespace RateLimiter.Implementations;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}