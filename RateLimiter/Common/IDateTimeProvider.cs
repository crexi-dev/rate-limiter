using System;

namespace RateLimiter.Common
{
    public interface IDateTimeProvider
    {
        DateTime GetUtcDate();
    }
}
