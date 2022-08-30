using System;

namespace RateLimiter.UtilityServices
{
    public interface IDateTimeProvider
    {
        DateTime GetDateTimeUtcNow();
    }
}
