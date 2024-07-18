using System;
using System.Collections.Generic;

namespace RateLimiter.RateLimitingRules;

public class RequestsPerTimeRule<TClient, TResource>(TimeProvider timeProvider, int numberOfRequests, TimeSpan lengthOfTime) : IRateLimitingRule<TClient, TResource>
{
    private readonly List<DateTimeOffset> _requestTimes = [];

    public bool HasReachedLimit(TClient client, TResource resource)
    {
        _requestTimes.RemoveAll(requestTime => timeProvider.GetUtcNow() - lengthOfTime >= requestTime);
        return _requestTimes.Count >= numberOfRequests;
    }

    public void RegisterRequest(TClient client, TResource resource)
    {
        _requestTimes.Add(timeProvider.GetUtcNow());
    }
}
