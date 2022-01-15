using System;

namespace RateLimiter.Common
{
    public sealed class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetUtcDate() => DateTime.UtcNow;
    }
}
