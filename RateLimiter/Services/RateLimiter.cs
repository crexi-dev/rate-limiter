using System;
using System.Collections.Generic;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Services;

/// <summary>
/// Class for managing rate limit rules.
/// </summary>
public class RateLimiter
{
    private readonly Dictionary<string, List<IRateLimitRule>> _resourceRules = new();
    private readonly Dictionary<string, List<IRateLimitRule>> _regionRules = new();

    public void AddRule(string resource, IRateLimitRule rule)
    {
        if (!_resourceRules.ContainsKey(resource))
        {
            _resourceRules[resource] = new List<IRateLimitRule>();
        }

        _resourceRules[resource].Add(rule);
    }

    public void AddRegionRule(string region, IRateLimitRule rule)
    {
        if (!_regionRules.ContainsKey(region))
        {
            _regionRules[region] = new List<IRateLimitRule>();
        }

        _regionRules[region].Add(rule);
    }

    public bool IsRequestAllowed(string clientToken, string resource, DateTime requestTime, string region)
    {
        if (_regionRules.ContainsKey(region))
        {
            foreach (var rule in _regionRules[region])
            {
                if (!rule.IsRequestAllowed(clientToken, resource, requestTime))
                {
                    return false;
                }
            }
        }

        if (_resourceRules.ContainsKey(resource))
        {
            foreach (var rule in _resourceRules[resource])
            {
                if (!rule.IsRequestAllowed(clientToken, resource, requestTime))
                {
                    return false;
                }
            }
        }

        return true;
    }
}