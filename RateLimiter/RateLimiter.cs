using System;
using System.Collections.Generic;
using RateLimiter.Library;
using RateLimiter.Library.Repository;

namespace RateLimiter
{
    public class RateLimiter : IRateLimiter
    {
        private IClientRepository clientRepository;
        private IRateLimiterAlgorithm rateLimiterAlgorithm;

        public RateLimiter(IClientRepository clientRepository,
            IRateLimiterAlgorithm rateLimiterAlgorithm) {
            this.clientRepository = clientRepository;
            this.rateLimiterAlgorithm = rateLimiterAlgorithm;
        }

        public bool Verify(string token, DateTime requestDate, RateLimitSettingsConfig rateLimitSettingsConfig)
        {
            var isAllowed = true;
            ClientRequestData clientData = new ClientRequestData(-1, DateTime.MinValue);

            // check requests per timespan rule
            if (rateLimitSettingsConfig[RateLimitType.RequestsPerTimespan] != null) {
                var rateLimitSettings = (TokenBucketSettings) rateLimitSettingsConfig[RateLimitType.RequestsPerTimespan];

                clientData = this.clientRepository.GetClientData(token);
                isAllowed = this.rateLimiterAlgorithm.VerifyRequestsPerTimeSpan(clientData.Count, rateLimitSettings.MaxAmount, rateLimitSettings.RefillAmount, rateLimitSettings.RefillTime, requestDate, clientData.LastUpdateDate);
            }

            if (!isAllowed)
                return false;

            // check timespan passed
            if (rateLimitSettingsConfig[RateLimitType.TimespanPassedSinceLastCall] != null)
            {
                var rateLimitSettings = (TimespanPassedSinceLastCallSettings) rateLimitSettingsConfig[RateLimitType.TimespanPassedSinceLastCall];

                if (clientData.Count == -1)
                    clientData = this.clientRepository.GetClientData(token);

                isAllowed = this.rateLimiterAlgorithm.VerifyTimespanPassedSinceLastCall(requestDate, rateLimitSettings.TimespanLimit, clientData.LastUpdateDate);
            }

            if (!isAllowed)
                return false;

            // update client data
            clientData.LastUpdateDate = DateTime.Now;
            this.clientRepository.AddOrUpdate(token, clientData);

            return true;
        }
    }
}