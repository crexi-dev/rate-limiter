using System;
using System.Collections.Generic;
using System.Net.Http;
using RateLimiter.ClientStatistics;
using RateLimiter.Interfaces;
using RateLimiter.Rules;

namespace RateLimiter.Handlers;

public class LastUsageRateLimitHandler : BaseRateLimitHandler<LastUsageRateLimitStatistics, LastUsageLimitRule>
{
    public LastUsageRateLimitHandler(LastUsageRateLimitStatistics statistics, List<LastUsageLimitRule> rules) : base(statistics, rules)
    {
    }
    
    public LastUsageRateLimitHandler(LastUsageRateLimitStatistics statistics, ITimeProvider timeProvider) 
        : base(statistics, new List<LastUsageLimitRule>(), timeProvider)
    {
    }
    
    public LastUsageRateLimitHandler(LastUsageRateLimitStatistics statistics) : base(statistics, new List<LastUsageLimitRule>())
    {
    }

    protected override void CalculateRuleFor(LastUsageLimitRule rule, IBucketIdentifier identifier)
    {
        var stat = Statistics.GetStatistics(identifier);
        if (stat == null)
            return;
        if (TimeProvider.Now.Subtract(stat.Value) < rule.Threshold)
            StopExecution();
        
    }
}