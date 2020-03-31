using System;
using RateLimiter.Library;
using RateLimiter.Library.Algorithms;
using RateLimiter.Library.Repository;

namespace RateLimiter.Client
{
    public class RateLimiterProxy : IRateLimiterProxy {
        public bool Verify(string token, DateTime requestDate, RateLimitSettingsConfig rateLimitSettingsConfig)
        {
            var clientRepository = new ClientRepository();
            var requestsPerTimeSpanRateLimiter = new TokenBucketRateLimiter();
            var timeSpanPassedSinceLastCallRateLimiter = new TimespanPassedSinceLastCallRateLimiter();
            var rateLimiterAlgorithm = new RateLimiterAlgorithm(requestsPerTimeSpanRateLimiter, timeSpanPassedSinceLastCallRateLimiter);
            var rateLimiter = new RateLimiter(clientRepository, rateLimiterAlgorithm);

            return rateLimiter.Verify(token, requestDate, rateLimitSettingsConfig);
        }
    }
}