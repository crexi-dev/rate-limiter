using System;

namespace RateLimiter.Client
{
    public class RateLimiterClient : IRateLimiterClient {
        private IRateLimiterProxy rateLimiterProxy;

        public RateLimiterClient(IRateLimiterProxy rateLimiterProxy) {
            this.rateLimiterProxy = rateLimiterProxy;
        }

        public bool VerifyTokenBucket(string token, DateTime requestDate, int count, int maxAmount, int refillAmount, int refillTime, DateTime lastUpdateDate) {
            return this.rateLimiterProxy.VerifyTokenBucket(token, requestDate, count, maxAmount, refillAmount, refillTime, lastUpdateDate);
        }

        public bool VerifyTimespanPassedSinceLastCall(string token, DateTime requestDate, TimeSpan timeSpanLimit)
        {
            return false;
        }
    }
}