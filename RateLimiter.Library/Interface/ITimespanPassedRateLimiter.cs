using System;

namespace RateLimiter.Library
{
    public interface ITimespanPassedSinceLastCallRateLimiter
    {
        bool VerifyTimespanPassedSinceLastCall(DateTime requestDate, TimeSpan timeSpanLimit, DateTime lastUpdateDate);
    }
}