using System;

namespace RateLimiter.Implementation
{
    public interface ITimespanPassedRateLimiter
    {
        bool Verify(DateTime requestDate, TimeSpan timeSpanLimit, DateTime lastUpdateDate);
    }
}