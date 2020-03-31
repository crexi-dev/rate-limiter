using System;

namespace RateLimiter.Library
{
    public abstract class RateLimitSettings {
    }

    public class RequestsPerTimespanSettings : RateLimitSettings {
        public int MaxAmount { get; set; }
        public int RefillAmount { get; set; }
        public int RefillTime { get; set; }
    }

    public class TimespanPassedSinceLastCallSettings : RateLimitSettings {
        public TimeSpan TimespanLimit { get; set; }
    }
}