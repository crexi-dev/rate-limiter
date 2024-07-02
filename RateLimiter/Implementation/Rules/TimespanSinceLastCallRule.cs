using System;

namespace RateLimiter.Implementation.Rules;

public class TimespanSinceLastCallRule : BaseRule
{
    private readonly TimeSpan _minimumInterval;

    public TimespanSinceLastCallRule(TimeSpan minimumInterval)
    {
        _minimumInterval = minimumInterval;
    }

    public override bool IsRequestAllowed(string clientId, string resource)
    {
        var lastRequestTime = GetLastRequestTime(clientId, resource);
        if (lastRequestTime != null && DateTime.UtcNow - lastRequestTime >= _minimumInterval)
        {
            SaveRequest(clientId, resource, DateTime.UtcNow);
            return true;
        }
        return false;
    }
}