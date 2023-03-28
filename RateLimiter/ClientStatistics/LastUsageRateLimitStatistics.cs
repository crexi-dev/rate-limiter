using System;
using System.Collections.Generic;
using RateLimiter.Interfaces;

namespace RateLimiter.ClientStatistics;

public class LastUsageRateLimitStatistics : IClientStatistics, IClientStatisticsProvider<DateTime?>
{
    private readonly Dictionary<IBucketIdentifier, DateTime> _lastRequest;

    public LastUsageRateLimitStatistics()
    {
        _lastRequest = new Dictionary<IBucketIdentifier, DateTime>();
    }

    public LastUsageRateLimitStatistics(Dictionary<IBucketIdentifier, DateTime> lastRequest)
    {
        _lastRequest = lastRequest;
    }

    public void IncrementStatisticsForRequest(IBucketIdentifier identifier, ITimeProvider timeProvider)
    {
        _lastRequest[identifier] = timeProvider.Now;
    }

    public DateTime? GetStatistics(IBucketIdentifier clientId)
    {
        if (_lastRequest.TryGetValue(clientId, out var dateTime))
            return dateTime;
        
        return null;
    }
}