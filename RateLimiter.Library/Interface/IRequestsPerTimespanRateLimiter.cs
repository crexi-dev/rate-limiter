using System;

namespace RateLimiter.Library
{
    public interface IRequestsPerTimeSpanRateLimiter {
        bool VerifyRequestsPerTimeSpan(int count, int maxAmount, int refillAmount, int refillTime, DateTime requestDate, DateTime lastUpdateDate);
    }
}