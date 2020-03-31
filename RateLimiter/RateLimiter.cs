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
        private RateLimitSettingsConfig rateLimitSettingsConfig;

        #region const
        private const int TOKEN_BUCKET_MAX_AMOUNT_DEFAULT = 5;
        private const int TOKEN_BUCKET_REFILL_TIME_DEFAULT = 60;
        private const int TOKEN_BUCKET_REFILL_AMOUNT_DEFAULT = 5;
        private const int TIMESPAN_PASSED_MINUTES_DEFAULT = 1;
        #endregion

        public RateLimiter(IClientRepository clientRepository,
            IRateLimiterAlgorithm rateLimiterAlgorithm) {
            this.clientRepository = clientRepository;
            this.rateLimiterAlgorithm = rateLimiterAlgorithm;
        }

        private void InitializeDefaultRateLimitSettings()
        {
            var defaultSettings = new Dictionary<RateLimitType, RateLimitSettingsBase>();

            var requestsPerTimespanSettings = new RequestsPerTimespanSettings()
            {
                MaxAmount = TOKEN_BUCKET_MAX_AMOUNT_DEFAULT,
                RefillAmount = TOKEN_BUCKET_REFILL_AMOUNT_DEFAULT,
                RefillTime = TOKEN_BUCKET_REFILL_TIME_DEFAULT
            };

            var timespanPassedSinceLastCallSettings = new TimespanPassedSinceLastCallSettings()
            {
                TimespanLimit = new TimeSpan(0, TIMESPAN_PASSED_MINUTES_DEFAULT, 0)
            };

            defaultSettings[RateLimitType.RequestsPerTimespan] = requestsPerTimespanSettings;
            defaultSettings[RateLimitType.TimespanPassedSinceLastCall] = timespanPassedSinceLastCallSettings;

            rateLimitSettingsConfig.RateLimitSettings = defaultSettings;
        }

        public bool Verify(string token, DateTime requestDate, RateLimitSettingsConfig rateLimitSettingsConfig = null)
        {
            var isAllowed = true;
            ClientRequestData clientData = new ClientRequestData(-1, DateTime.MinValue, DateTime.MinValue);

            // check requests per timespan rule
            if (rateLimitSettingsConfig.RateLimitSettings.ContainsKey(RateLimitType.RequestsPerTimespan)) {
                var rateLimitSettings = rateLimitSettingsConfig != null 
                    ? (RequestsPerTimespanSettings) this.rateLimitSettingsConfig.RateLimitSettings[RateLimitType.RequestsPerTimespan]
                    : (RequestsPerTimespanSettings) rateLimitSettingsConfig.RateLimitSettings[RateLimitType.RequestsPerTimespan];

                clientData = this.clientRepository.GetClientData(token);
                isAllowed = this.rateLimiterAlgorithm.VerifyRequestsPerTimeSpan(clientData.Count, rateLimitSettings.MaxAmount, rateLimitSettings.RefillAmount, rateLimitSettings.RefillTime, requestDate, clientData.LastUpdateDate);
            }

            if (!isAllowed)
                return false;

            // check timespan passed
            if (rateLimitSettingsConfig.RateLimitSettings.ContainsKey(RateLimitType.TimespanPassedSinceLastCall))
            {
                var rateLimitSettings = rateLimitSettingsConfig != null
                    ? (TimespanPassedSinceLastCallSettings) this.rateLimitSettingsConfig.RateLimitSettings[RateLimitType.TimespanPassedSinceLastCall]
                    : (TimespanPassedSinceLastCallSettings) rateLimitSettingsConfig.RateLimitSettings[RateLimitType.RequestsPerTimespan];

                if (clientData.Count != -1)
                    clientData = this.clientRepository.GetClientData(token);

                isAllowed = this.rateLimiterAlgorithm.VerifyTimespanPassedSinceLastCall(requestDate, rateLimitSettings.TimespanLimit, clientData.LastUpdateDate);
            }

            if (!isAllowed)
                return false;

            // update client data
            clientData.LastUpdateDate = DateTime.Now;
            this.clientRepository.UpdateClient(token, clientData);

            return true;
        }
    }
}