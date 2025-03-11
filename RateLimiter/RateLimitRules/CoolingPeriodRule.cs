using System;
using System.Collections.Generic;

namespace RateLimiter.RateLimitRules;

public class CoolingPeriodRule: IRateLimitRule
{
    private readonly TimeSpan _minimumInterval;
    
    private readonly Dictionary<(string clientId, string resource), DateTime> _lastRequest = new();

    public CoolingPeriodRule(TimeSpan minimumInterval)
    {
        _minimumInterval = minimumInterval;
    }

    public bool IsRequestAllowed(RequestContext context, DateTime requestTime)
    {
        var key = (context.ClientId, context.Resource);
        if (_lastRequest.TryGetValue(key, out var lastTime))
        {
            if (requestTime - lastTime < _minimumInterval)
            {
                return false;
            }
        }

        _lastRequest[key] = requestTime;
        return true;
    }
}