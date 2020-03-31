using System;

namespace RateLimiter.Library
{
    public class RateLimiterAlgorithm : IRateLimiterAlgorithm
    {
        private IRequestsPerTimeSpanRateLimiter requestsPerTimeSpanRateLimiter;
        private ITimespanPassedSinceLastCallRateLimiter timespanPassedSinceLastCallRateLimiter;

        public RateLimiterAlgorithm(IRequestsPerTimeSpanRateLimiter requestsPerTimeSpanRateLimiter,
            ITimespanPassedSinceLastCallRateLimiter timespanPassedSinceLastCallRateLimiter)
        {
            this.requestsPerTimeSpanRateLimiter = requestsPerTimeSpanRateLimiter;
            this.timespanPassedSinceLastCallRateLimiter = timespanPassedSinceLastCallRateLimiter;
        }

        // requests per timespan
        public bool VerifyRequestsPerTimeSpan(int count, int maxAmount, int refillAmount, int refillTime, DateTime requestDate, DateTime lastUpdateDate)
        {
            return this.requestsPerTimeSpanRateLimiter.VerifyRequestsPerTimeSpan(count, maxAmount, refillAmount, refillTime, requestDate, lastUpdateDate);
        }

        // timespan passed
        public bool VerifyTimespanPassedSinceLastCall(DateTime requestDate, TimeSpan timeSpanLimit, DateTime lastUpdateDate)
        {
            return this.timespanPassedSinceLastCallRateLimiter.VerifyTimespanPassedSinceLastCall(requestDate, timeSpanLimit, lastUpdateDate);
        }
    }
}