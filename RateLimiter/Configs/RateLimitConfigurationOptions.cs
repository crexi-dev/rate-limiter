using System;
using System.Collections.Generic;
using System.Text;
using RateLimiter.Rules;

namespace RateLimiter.Configs
{
    public class RateLimitConfigurationOptions
    {
        public List<NumberOfRequestsPerTimespan> NumberOfRequestsPerTimespanRules { get; set; }

        public List<TimeSpanBetweenTwoRequests> TimeSpanBetweenTwoRequestsRules { get; set; }

        public TimeSpan? ExpirationTime { get; set; }

        // By default rate limit rules are enabled
        public bool Enabled { get; set; } = true;
    }
}
