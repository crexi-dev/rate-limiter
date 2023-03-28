using System;
using RateLimiter.Interfaces;

namespace RateLimiter.Rules;

public class PeriodRateLimitRule : BaseRateLimitRule, IRateLimitRuleThresholdProvider<int>
{
    public PeriodRateLimitRule(IMatcher matchers, TimeSpan period, int threshold) : base(matchers)
    {
        Period = period;
        Threshold = threshold;
    }
    
    public TimeSpan Period;

    public int Threshold { get; set; }
}