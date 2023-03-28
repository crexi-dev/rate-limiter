using System;
using System.Collections.Generic;
using RateLimiter.Interfaces;

namespace RateLimiter.ClientStatistics;

public class PeriodTotalRequestRateLimitStatistics : IClientStatistics, IClientStatisticsProvider<LinkedList<DateTime>>
{
    private readonly Dictionary<IBucketIdentifier, LinkedList<DateTime>> _statistics;

    public PeriodTotalRequestRateLimitStatistics(Dictionary<IBucketIdentifier, LinkedList<DateTime>> statistics)
    {
        _statistics = statistics;
    }

    public PeriodTotalRequestRateLimitStatistics()
    {
        _statistics = new Dictionary<IBucketIdentifier, LinkedList<DateTime>>();
    }


    public void IncrementStatisticsForRequest(IBucketIdentifier clientId, ITimeProvider timeProvider)
    {
        if (!_statistics.TryGetValue(clientId, out var list))
        {
            _statistics[clientId] = list = new LinkedList<DateTime>();
        }

        list.AddFirst(DateTime.Now);
    }

    public LinkedList<DateTime> GetStatistics(IBucketIdentifier clientId)
    {
        return _statistics.TryGetValue(clientId, out var list) ? list : new LinkedList<DateTime>();
    }

    internal void TruncatePeriod(DateTime oldestPeriod)
    {
        foreach (var stat in _statistics)
        {
            while (stat.Value.Last != null && stat.Value.Last.Value < oldestPeriod)
            {
                stat.Value.RemoveLast();
            }
        }
    }
}