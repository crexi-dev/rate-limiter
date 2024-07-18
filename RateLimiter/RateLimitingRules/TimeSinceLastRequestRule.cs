using System;

namespace RateLimiter.RateLimitingRules;

/// <summary>
/// A rate limiting rule which will limit a single client / resource combination to one request for a length of time.
/// </summary>
/// <typeparam name="TClient">The type of client (who is requesting a resource).</typeparam>
/// <typeparam name="TResource">The type of resource (what thing is being accessed).</typeparam>
/// <param name="timeProvider">See <see cref="TimeProvider"/></param>
/// <param name="lengthOfTime">The length of time.</param>
public class TimeSinceLastRequestRule<TClient, TResource>(TimeProvider timeProvider, TimeSpan lengthOfTime) : IRateLimitingRule<TClient, TResource>
{
    private DateTimeOffset _lastCall = DateTimeOffset.MinValue;

    /// <inheritdoc/>
    public bool HasReachedLimit(TClient client, TResource resource)
    {
        return _lastCall > timeProvider.GetUtcNow() - lengthOfTime;
    }

    /// <inheritdoc/>
    public void RegisterRequest(TClient client, TResource resource)
    {
        _lastCall = timeProvider.GetUtcNow();
    }
}
