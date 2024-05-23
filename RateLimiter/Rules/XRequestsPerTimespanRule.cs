using System;
using System.Collections.Generic;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Rules;

public class XRequestsPerTimespanRule : IRateLimitRule
{
    private readonly int _maxRequests;
    private readonly TimeSpan _timespan;
    private readonly Dictionary<string, List<DateTime>> _clientRequests = new();

    public XRequestsPerTimespanRule(int maxRequests, TimeSpan timespan)
    {
        _maxRequests = maxRequests;
        _timespan = timespan;
    }

    public bool IsRequestAllowed(string clientToken, string resource, DateTime requestTime)
    {
        if (!_clientRequests.ContainsKey(clientToken))
        {
            _clientRequests[clientToken] = new List<DateTime>();
        }

        _clientRequests[clientToken].RemoveAll(r => r < requestTime - _timespan);

        if (_clientRequests[clientToken].Count >= _maxRequests)
        {
            return false;
        }

        _clientRequests[clientToken].Add(requestTime);
        return true;
    }
}
