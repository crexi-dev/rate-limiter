using System;
using RateLimiter.Library.Repository;

namespace RateLimiter.Client
{
    public class RateLimiterProxy : IRateLimiterProxy {
        public bool VerifyTokenBucket(string token, DateTime requestDate, int count, int maxAmount, int refillAmount, int refillTime, DateTime lastUpdateDate)
        {
            var clientRepository = new ClientRepository();
            var rateLimiter = new RateLimiter(clientRepository);

            return rateLimiter.VerifyTimespanPassedSinceLastCall(token, requestDate, new TimeSpan(0, 0, 0));
        }

        public bool VerifyTimespanPassedSinceLastCall(string token, DateTime requestDate, TimeSpan timeSpanLimit)
        {
            return false;
        }
    }
}