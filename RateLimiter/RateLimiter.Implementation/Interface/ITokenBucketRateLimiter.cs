using System;

namespace RateLimiter.Implementation
{
    public interface ITokenBucketRateLimiter {
        bool Verify(int count, int maxAmount, int refillAmount, int refillTime, DateTime requestDate, DateTime lastUpdateDate);
    }
}