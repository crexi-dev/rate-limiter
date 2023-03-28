using System;
using RateLimiter.Interfaces;

namespace RateLimiter.Rules;

public class LastUsageLimitRule : BaseRateLimitRule, IRateLimitRuleThresholdProvider<TimeSpan>
{
    public LastUsageLimitRule(IMatcher matchers, TimeSpan threshold) : base(matchers)
    {
        Threshold = threshold;
    }


    public TimeSpan Threshold { get; init; }
}