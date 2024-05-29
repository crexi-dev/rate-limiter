using System;

namespace RateLimiter.Interfaces
{
    public interface IDateTimeWrapper
    {
        DateTime UtcNow { get; }
    }
}
