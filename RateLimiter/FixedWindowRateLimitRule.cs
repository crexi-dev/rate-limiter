using System.Collections.Generic;
using System;

namespace RateLimiter;

public class FixedWindowRateLimitRule : IRateLimitRule
{
    private readonly int _requestLimit;
    private readonly TimeSpan _windowSize;
    private readonly Dictionary<string, (DateTime startTime, int requestCount)> _requestLog = new();

    public FixedWindowRateLimitRule(int requestLimit, TimeSpan windowSize)
    {
        _requestLimit = requestLimit;
        _windowSize = windowSize;
    }

    public bool IsRequestAllowed(string token)
    {
        if (token is null)
        {
            return false;
        }

        if (!_requestLog.ContainsKey(token))
        {
            _requestLog[token] = (DateTime.UtcNow, 1);
            return true;
        }

        var (startTime, requestCount) = _requestLog[token];
        if (DateTime.UtcNow - startTime > _windowSize)
        {
            _requestLog[token] = (DateTime.UtcNow, 1);
            return true;
        }
        else if (requestCount < _requestLimit)
        {
            _requestLog[token] = (startTime, requestCount + 1);
            return true;
        }

        return false;
    }
}