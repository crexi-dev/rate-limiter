using System;
using RateLimiter.Library;

namespace RateLimiter.RulesEngine
{
    public class DefaultRateLimiterSettings
    {
        public RateLimitSettingsConfig RateLimiterSettings { get; set; }

        public DefaultRateLimiterSettings()
        {
            this.RateLimiterSettings[RateLimitType.RequestsPerTimespan] = new TokenBucketSettings()
            {
                MaxAmount = 5,
                RefillAmount = 5,
                RefillTime = 60
            };

            this.RateLimiterSettings[RateLimitType.TimespanPassedSinceLastCall] = new TimespanPassedSinceLastCallSettings()
            {
                TimespanLimit = new TimeSpan(0, 1, 0)
            };
        }

        public RateLimitSettingsBase this[RateLimitType rateLimitType]
        {
            get
            {
                return this.RateLimiterSettings[rateLimitType];
            }
        }
    }
}