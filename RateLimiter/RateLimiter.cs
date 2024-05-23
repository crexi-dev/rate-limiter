using System;
using System.Collections.Generic;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter;

public class RateLimiter
{
    private readonly Dictionary<string, List<IRateLimitRule>> _resourceRules = new();

    public void AddRule(string resource, IRateLimitRule rule)
    {
        if (!_resourceRules.ContainsKey(resource))
        {
            _resourceRules[resource] = new List<IRateLimitRule>();
        }

        _resourceRules[resource].Add(rule);
    }

    public bool IsRequestAllowed(string clientToken, string resource, DateTime requestTime)
    {
        if (!_resourceRules.ContainsKey(resource))
        {
            return true;
        }

        foreach (var rule in _resourceRules[resource])
        {
            if (!rule.IsRequestAllowed(clientToken, resource, requestTime))
            {
                return false;
            }
        }

        return true;
    }
}