using System;
using RateLimiter.Interfaces;
using RateLimiter.Rules;

namespace RateLimiter;

public class RuleFactory
{
    public PeriodRateLimitRule CreateRule(RequestCountConfiguration option)
    {
        return new PeriodRateLimitRule(MatchersFactory.CreateMatchers(option), option.Duration, option.Count);
    }
    
    public LastUsageLimitRule CreateRule(LastUsageConfiguration option)
    {
        return new LastUsageLimitRule(MatchersFactory.CreateMatchers(option), option.DelayTime);
    }
    
}