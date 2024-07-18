using System;
using System.Collections.Generic;

namespace RateLimiter.RateLimitingRules;

/// <summary>
/// A rate limiting rule which will limit a single client / resource combination to a number of requests for a length of time.
/// </summary>
/// <typeparam name="TClient">The type of client (who is requesting a resource).</typeparam>
/// <typeparam name="TResource">The type of resource (what thing is being accessed).</typeparam>
/// <param name="timeProvider">See <see cref="TimeProvider"/></param>
/// <param name="numberOfRequests">The number of requests to allow for the length of time.</param>
/// <param name="lengthOfTime">The length of time.</param>
public class RequestsPerTimeRule<TClient, TResource>(TimeProvider timeProvider, int numberOfRequests, TimeSpan lengthOfTime) : IRateLimitingRule<TClient, TResource>
{
    private readonly List<DateTimeOffset> _requestTimes = [];

    /// <inheritdoc/>
    public bool HasReachedLimit(TClient client, TResource resource)
    {
        _requestTimes.RemoveAll(requestTime => timeProvider.GetUtcNow() - lengthOfTime >= requestTime);
        return _requestTimes.Count >= numberOfRequests;
    }

    /// <inheritdoc/>
    public void RegisterRequest(TClient client, TResource resource)
    {
        _requestTimes.Add(timeProvider.GetUtcNow());
    }
}
