using System;
using System.Diagnostics.CodeAnalysis;

namespace RateLimiter.UtilityServices
{
    [ExcludeFromCodeCoverage]
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetDateTimeUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
