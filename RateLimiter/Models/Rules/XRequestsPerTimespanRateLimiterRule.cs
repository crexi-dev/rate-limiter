using System;
using System.Diagnostics.CodeAnalysis;
using RateLimiter.Models.Enums;

namespace RateLimiter.Models.Rules
{
    [ExcludeFromCodeCoverage]
    public class XRequestsPerTimespanRateLimiterRule : RateLimiterRuleBase
    {
        public TimeSpan TimeSpanPeriod { get; set; }
        public int RequestsLimit { get; set; }

        public override RateLimiterType RateLimiterType => RateLimiterType.XRequestsPerTimespan;
    }
}
