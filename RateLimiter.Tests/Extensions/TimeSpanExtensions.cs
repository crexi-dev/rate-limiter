using System;

namespace RateLimiter.Tests.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan AddMilliseconds(this TimeSpan timeSpan, int milliseconds)
        {
            return new TimeSpan(0, 0, 0, 0, (int)timeSpan.TotalMilliseconds + milliseconds);
        }

        public static TimeSpan FromSeconds(int seconds)
        {
            return new TimeSpan(0, 0, 0, seconds);
        }

        public static TimeSpan FromMilliseconds(int milliseconds)
        {
            return new TimeSpan(0, 0, 0, 0, milliseconds);
        }
    }
}
