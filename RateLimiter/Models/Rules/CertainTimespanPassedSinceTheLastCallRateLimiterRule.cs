using System;
using System.Diagnostics.CodeAnalysis;
using RateLimiter.Models.Enums;

namespace RateLimiter.Models.Rules
{
    [ExcludeFromCodeCoverage]
    public class CertainTimespanPassedSinceTheLastCallRateLimiterRule : RateLimiterRuleBase
    {
        public TimeSpan TimeSpanPeriod { get; set; }

        public override RateLimiterType RateLimiterType => RateLimiterType.CertainTimespanPassedSinceTheLastCall;
    }
}
