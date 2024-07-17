using System;

namespace RateLimiter.RateLimitingRules;

public class TimeSinceLastRequestRule<TClient, TResource>(TimeProvider timeProvider, TimeSpan lengthOfTime) : IRateLimitingRule<TClient, TResource>
{
    private DateTimeOffset _lastCall = DateTimeOffset.MinValue;

    public bool HasExceededLimit(TClient client, TResource resource)
    {
        return _lastCall > timeProvider.GetUtcNow() - lengthOfTime;
    }

    public void RegisterRequest(TClient client, TResource resource)
    {
        _lastCall = timeProvider.GetUtcNow();
    }
}
