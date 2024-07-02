using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter;

public abstract class BaseRule : IRateLimitingRule
{
    private static readonly ConcurrentDictionary<string, List<DateTime>> Requests = new();

    public abstract bool IsRequestAllowed(string clientId, string resource);

    public virtual DateTime? GetLastRequestTime(string clientId, string resource)
    {
        var key = $"{clientId}:{resource}";
        if (Requests.TryGetValue(key, out var timestamps) && timestamps.Any())
        {
            return timestamps.Last();
        }
        return null;
    }

    protected int GetRequestCount(string clientId, string resource, DateTime start, DateTime end)
    {
        var key = $"{clientId}:{resource}";
        if (Requests.TryGetValue(key, out var timestamps))
        {
            return timestamps.Count(timestamp => timestamp >= start && timestamp <= end);
        }
        return 0;
    }

    public void SaveRequest(string clientId, string resource, DateTime timestamp)
    {
        var key = $"{clientId}:{resource}";
        Requests.AddOrUpdate(key, new List<DateTime> { timestamp }, (_, timestampList) =>
        {
            timestampList.Add(timestamp);
            return timestampList;
        });
    }
}