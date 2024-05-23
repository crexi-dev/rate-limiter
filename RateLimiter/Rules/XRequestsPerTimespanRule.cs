using System;
using System.Collections.Generic;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Rules;

/// <summary>
/// Rule to limit the number of requests per timespan.
/// </summary>
public class XRequestsPerTimespanRule : IRateLimitRule
{
    private readonly int _maxRequests;
    private readonly TimeSpan _timespan;
    private readonly Dictionary<string, Queue<DateTime>> _clientRequests = new();

    public XRequestsPerTimespanRule(int maxRequests, TimeSpan timespan)
    {
        _maxRequests = maxRequests;
        _timespan = timespan;
    }

    public bool IsRequestAllowed(string clientToken, string resource, DateTime requestTime)
    {
        if (!_clientRequests.ContainsKey(clientToken))
        {
            _clientRequests[clientToken] = new Queue<DateTime>();
        }

        var requests = _clientRequests[clientToken];

        // Remove requests that are outside the timespan window
        while (requests.Count > 0 && requests.Peek() < requestTime - _timespan)
        {
            requests.Dequeue();
        }

        if (requests.Count >= _maxRequests)
        {
            return false;
        }

        // Add the current request
        requests.Enqueue(requestTime);
        return true;
    }
}