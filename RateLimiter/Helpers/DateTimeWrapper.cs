using RateLimiter.Interfaces;
using System;

namespace RateLimiter.Helpers
{
    /// <inheritdoc />
    public class DateTimeWrapper : IDateTimeWrapper
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
