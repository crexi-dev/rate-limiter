using System;

namespace RateLimiter.Library
{
    public interface ITimespanPassedRateLimiter
    {
        bool Verify(DateTime requestDate, TimeSpan timeSpanLimit, DateTime lastUpdateDate);
    }
}