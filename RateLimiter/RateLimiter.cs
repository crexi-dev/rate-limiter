using System.Collections.Generic;

namespace RateLimiter;

public class RateLimiter
{
    private readonly Dictionary<string, List<IRateLimitRule>> _defaultResourceRules = new();
    private readonly Dictionary<string, Dictionary<string, List<IRateLimitRule>>> _specificLocationResourceRules = new();

    public void AddDefaultRuleForResource(string resource, IRateLimitRule rule)
    {
        if (!_defaultResourceRules.ContainsKey(resource))
        {
            _defaultResourceRules[resource] = new List<IRateLimitRule>();
        }

        _defaultResourceRules[resource].Add(rule);
    }

    public void AddRuleForTokenAndResource(string location, string resource, IRateLimitRule rule)
    {
        if (!_specificLocationResourceRules.ContainsKey(resource))
        {
            _specificLocationResourceRules[resource] = new Dictionary<string, List<IRateLimitRule>>();
        }

        if (!_specificLocationResourceRules[resource].ContainsKey(location))
        {
            _specificLocationResourceRules[resource][location] = new List<IRateLimitRule>();
        }

        _specificLocationResourceRules[resource][location].Add(rule);
    }

    public bool IsRequestAllowed(string resource, string location, string token)
    {
        if (_specificLocationResourceRules.ContainsKey(resource)
            && location is not null
            && _specificLocationResourceRules[resource].ContainsKey(location))
        {
            foreach (var rule in _specificLocationResourceRules[resource][location])
            {
                if (!rule.IsRequestAllowed(token))
                {
                    return false;
                }
            }
        }
        else if (_defaultResourceRules.ContainsKey(resource))
        {
            foreach (var rule in _defaultResourceRules[resource])
            {
                if (!rule.IsRequestAllowed(token))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
