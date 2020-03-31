using System;

namespace RateLimiter.Library
{
    public interface ITokenBucketRateLimiter {
        bool Verify(int count, int maxAmount, int refillAmount, int refillTime, DateTime requestDate, DateTime lastUpdateDate);
    }
}