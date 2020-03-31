using System;
using RateLimiter.Library;
using RateLimiter.Library.Repository;

namespace RateLimiter
{
    public class RateLimiter : IRateLimiter
    {
        private IClientRepository clientRepository;

        public RateLimiter(IClientRepository clientRepository) {
            this.clientRepository = clientRepository;
        }

        public bool Verify(RateLimitType rateLimitType, RequestsPerTimespanSettings requestsPerTimespanSettings = null, TimespanPassedSinceLastCallSettings timespan = null)
        {
            var isAllowed = true;

            if ((rateLimitType & RateLimitType.RequestsPerTimespan) != 0)
            {

            }

            if ((rateLimitType & RateLimitType.TimespanPassedSinceLastCall) != 0)
            {

            }

            // if call is allowed, update client count

            return false;
        }

        public bool VerifyTokenBucket(string token, DateTime requestDate, int count, int maxAmount, int refillAmount, int refillTime, DateTime lastUpdateDate)
        {
            // retrieve client data
            var clientData = this.clientRepository.GetClientData(token);

            // verify w/in rate limits (invoke appropriate rate limiter methods)

            var isAllowed = false;

            if (!isAllowed)
                return false;
            
            else {
                // update client data

                return true;
            }
        }

        public bool VerifyTimespanPassedSinceLastCall(string token, DateTime requestDate, TimeSpan timeSpanLimit)
        {
            return false;
        }
    }
}