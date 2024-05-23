using System;
using System.Collections.Generic;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Rules;

public class CertainTimespanPassedRule : IRateLimitRule
{
    private readonly TimeSpan _requiredTimespan;
    private readonly Dictionary<string, DateTime> _clientLastRequestTime = new();

    public CertainTimespanPassedRule(TimeSpan requiredTimespan)
    {
        _requiredTimespan = requiredTimespan;
    }

    public bool IsRequestAllowed(string clientToken, string resource, DateTime requestTime)
    {
        if (!_clientLastRequestTime.ContainsKey(clientToken))
        {
            _clientLastRequestTime[clientToken] = DateTime.MinValue;
        }

        if (requestTime - _clientLastRequestTime[clientToken] < _requiredTimespan)
        {
            return false;
        }

        _clientLastRequestTime[clientToken] = requestTime;
        return true;
    }
}