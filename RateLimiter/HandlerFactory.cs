using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.ClientStatistics;
using RateLimiter.Handlers;
using RateLimiter.Interfaces;

namespace RateLimiter;

public class HandlerFactory
{
    private RuleFactory _ruleFactory;
    private IStatisticStorageProvider _statisticStorage;
    private ITimeProvider _timeProvider;

    public HandlerFactory(RuleFactory ruleFactory, IStatisticStorageProvider statisticStorage, ITimeProvider timeProvider)
    {
        _ruleFactory = ruleFactory;
        _statisticStorage = statisticStorage;
        _timeProvider = timeProvider;
    }


    public HandlerFactory() : this(new RuleFactory(), new StatisticsStorage(), new TimeProvider())
    {
    }

    public ICollection<IRateLimitHandler> CreateHandlers(List<RateLimitConfiguration> rules)
    {
        var result = new List<IRateLimitHandler>();

        foreach (var ruleList in rules.GroupBy(c => c.Type))
        {
            switch (ruleList.Key)
            {
                case RuleType.RequestCount:
                    var rateLimitHandler = new RateLimitHandler(_statisticStorage.GetStorage<PeriodTotalRequestRateLimitStatistics>(), _timeProvider);
                    foreach (var option in ruleList)
                    {
                        rateLimitHandler.AddRule(_ruleFactory.CreateRule(option as RequestCountConfiguration ?? throw new ArgumentException()));
                    }
                    result.Add(rateLimitHandler);
                    break;
                case RuleType.LastUsage:
                    var lastUsageHandler = new LastUsageRateLimitHandler(_statisticStorage.GetStorage<LastUsageRateLimitStatistics>(), _timeProvider);
                    foreach (var option in ruleList)
                    {
                        lastUsageHandler.AddRule(_ruleFactory.CreateRule(option as LastUsageConfiguration ?? throw new ArgumentException()));
                    }
                    result.Add(lastUsageHandler);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return result;

    }
}