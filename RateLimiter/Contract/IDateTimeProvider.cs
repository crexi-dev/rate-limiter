using System;

namespace RateLimiter.Contract
{
    internal interface IDateTimeProvider
    {
        DateTime Now { get; }

        DateTime UtcNow { get; }
    }
}
