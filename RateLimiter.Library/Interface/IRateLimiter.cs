using System;
using RateLimiter.Library;

namespace RateLimiter.Library
{
    public interface IRateLimiter
    {
        bool Verify(RateLimitType rateLimitType, RequestsPerTimespanSettings requestsPerTimespanSettings = null, TimespanPassedSinceLastCallSettings timespanPassedSinceLastCallSettings = null);
        bool VerifyTokenBucket(string token, DateTime requestDate, int count, int maxAmount, int refillAmount, int refillTime, DateTime lastUpdateDate);
        bool VerifyTimespanPassedSinceLastCall(string token, DateTime requestDate, TimeSpan timeSpanLimit);
    }
}