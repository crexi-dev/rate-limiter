using System;
using System.Collections.Generic;

namespace RateLimiter.Library
{
    public abstract class RateLimitSettingsBase
    {

    }

    public class RateLimitSettingsConfig {
        public Dictionary<RateLimitType, RateLimitSettingsBase> RateLimitSettings { get; set; }

        public RateLimitSettingsConfig()
        {
            this.RateLimitSettings = new Dictionary<RateLimitType, RateLimitSettingsBase>();
        }

        public RateLimitSettingsBase this[RateLimitType rateLimitType]
        {
            get
            {
                return RateLimitSettings[rateLimitType];
            }
            set
            {
                RateLimitSettings[rateLimitType] = value;
            }
        }
    }

    public class TokenBucketSettings : RateLimitSettingsBase {
        public int MaxAmount { get; set; }
        public int RefillAmount { get; set; }
        public int RefillTime { get; set; }
    }

    public class TimespanPassedSinceLastCallSettings : RateLimitSettingsBase {
        public TimeSpan TimespanLimit { get; set; }
    }
}