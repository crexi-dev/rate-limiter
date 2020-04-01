using System;

namespace RateLimiter.Library
{
    [Flags]
    public enum RateLimitType {
        Whitelist = 0,
        RequestsPerTimespan = 1,
        TimespanPassedSinceLastCall = 2
    }    
}