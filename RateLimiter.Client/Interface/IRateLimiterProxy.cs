using System;

namespace RateLimiter.Client
{
    public interface IRateLimiterProxy {
        bool VerifyTokenBucket(string token, DateTime requestDate, int count, int maxAmount, int refillAmount, int refillTime, DateTime lastUpdateDate);
        bool VerifyTimespanPassedSinceLastCall(string token, DateTime requestDate, TimeSpan timeSpanLimit);
    }
}