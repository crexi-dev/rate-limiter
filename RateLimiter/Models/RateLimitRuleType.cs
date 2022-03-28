using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models
{
    public enum RateLimitRuleType
    {
        RequestsPerTimespan = 1,
        TimespanPassedSinceLastCall = 2
    }
}
