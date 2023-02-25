using System;

namespace RateLimiter
{
    public interface ITimeService
    {
        DateTime GetCurrentTime { get; }
    }
}