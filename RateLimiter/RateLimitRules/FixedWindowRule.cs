using System;
using System.Collections.Generic;

namespace RateLimiter.RateLimitRules;

public class FixedWindowRule: IRateLimitRule
{
    private readonly int _maxRequests;
    private readonly TimeSpan _windowSize;

    private readonly Dictionary<(string clientId, string resource), List<DateTime>> _requestsInfo = new();

    public FixedWindowRule(int maxRequests, TimeSpan windowSize)
    {
        _maxRequests = maxRequests;
        _windowSize = windowSize;
    }

    public bool IsRequestAllowed(RequestContext context, DateTime requestTime)
    {
        var key = (context.ClientId, context.Resource);

        if (!_requestsInfo.ContainsKey(key))
        {
            _requestsInfo[key] = new List<DateTime>();
        }

        var earliestAllowedTime = requestTime - _windowSize;
        _requestsInfo[key].RemoveAll(dt => dt < earliestAllowedTime);

        if (_requestsInfo[key].Count >= _maxRequests)
        {
            return false;
        }

        _requestsInfo[key].Add(requestTime);
        return true;
    }
}