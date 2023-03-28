using System;

namespace RateLimiter.Interfaces;

public interface ITimeProvider
{
    public DateTime Now { get; }
}