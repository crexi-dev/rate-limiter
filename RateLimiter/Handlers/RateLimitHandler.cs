using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using RateLimiter.ClientStatistics;
using RateLimiter.Interfaces;
using RateLimiter.Rules;

namespace RateLimiter.Handlers;

public class RateLimitHandler : BaseRateLimitHandler<PeriodTotalRequestRateLimitStatistics, PeriodRateLimitRule>
{
    private TimeSpan _maxPeriod;

    public RateLimitHandler(PeriodTotalRequestRateLimitStatistics statistics) : base(statistics, new List<PeriodRateLimitRule>())
    {
    }

    public RateLimitHandler(PeriodTotalRequestRateLimitStatistics statistics, ITimeProvider timeProvider) 
        : base(statistics, new List<PeriodRateLimitRule>(), timeProvider)
    {
    }
    
    public RateLimitHandler(PeriodTotalRequestRateLimitStatistics statistics, List<PeriodRateLimitRule> rules) : base(statistics, rules)
    {
        _maxPeriod = Rules.Max(r => r.Period);
    }
    
    public override void AddRule(PeriodRateLimitRule rule)
    {
        Rules.Add(rule);
        _maxPeriod = Rules.Max(r => r.Period);
    }
    
    protected override void CalculateRuleFor(PeriodRateLimitRule rule, IBucketIdentifier identifier)
    {
        var stat = Statistics.GetStatistics(identifier);
        var startDate = TimeProvider.Now.Subtract(rule.Period);
        var countOfRequest = stat.TakeWhile(c => c > startDate).Count();
        stat.AddFirst(TimeProvider.Now);
            
        if (countOfRequest > rule.Threshold)
        {
            StopExecution();
        }
    }

    protected override void UpdateStatisticFor(RequestInformation request)
    {
        Statistics.TruncatePeriod(TimeProvider.Now.Subtract(_maxPeriod));
    }
}