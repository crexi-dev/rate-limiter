using System;

namespace RateLimiter.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}