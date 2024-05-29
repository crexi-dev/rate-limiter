using RateLimiter.Interfaces;
using System;

namespace RateLimiter.Helpers
{
    public class DateTimeWrapper : IDateTimeWrapper
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
