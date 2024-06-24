using System.Collections.Generic;
using System;

namespace RateLimiter;

public class TimeSinceLastCallRule : IRateLimitRule
{
    private readonly TimeSpan _minimumInterval;
    private readonly Dictionary<string, DateTime> _lastRequestTime = new();

    public TimeSinceLastCallRule(TimeSpan minimumInterval)
    {
        _minimumInterval = minimumInterval;
    }

    public bool IsRequestAllowed(string token)
    {
        if (token is null)
        {
            return false;
        }

        if (!_lastRequestTime.ContainsKey(token) || DateTime.UtcNow - _lastRequestTime[token] > _minimumInterval)
        {
            _lastRequestTime[token] = DateTime.UtcNow;
            return true;
        }

        return false;
    }
}
