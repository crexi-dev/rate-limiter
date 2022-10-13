using RateLimiter.Contract;
using System;

namespace RateLimiter
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;

        public DateTime UtcNow => DateTime.UtcNow;
    }
}
