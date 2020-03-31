using System;

namespace RateLimiter.Library.Algorithms
{
    public class TokenBucketRateLimiter : IRequestsPerTimeSpanRateLimiter {
        public bool VerifyRequestsPerTimeSpan(int count, int maxAmount, int refillAmount, int refillTime, DateTime requestDate, DateTime lastUpdateDate) {
            // refill
            var refillCount = (int) Math.Floor((double)(requestDate - lastUpdateDate).TotalSeconds / refillTime);

            count = Math.Min(maxAmount, count + refillCount * refillAmount);

            // check if token is available
            if (count < 1)
                return false;
            
            return true;
        }
    }
}