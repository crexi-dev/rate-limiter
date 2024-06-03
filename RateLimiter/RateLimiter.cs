using System;
using System.Collections.Generic;
using System.Linq;

public class RateLimiter
{
    private readonly Dictionary<string, List<IRateLimitRule>> _resourceRules = [];
    private readonly Dictionary<(string client, string resource), List<DateTime>> _clientRequests = [];

    public void AddRule(string resource, IRateLimitRule rule)
    {
        if (!_resourceRules.ContainsKey(resource))
        {
            _resourceRules[resource] = new List<IRateLimitRule>();
        }
        _resourceRules[resource].Add(rule);
    }

    public bool IsRequestAllowed(string client, string resource)
    {
        var currentTime = DateTime.UtcNow;
        if (!_resourceRules.ContainsKey(resource))
        {
            return true;
        }

        var rules = _resourceRules[resource];
        return rules.All(rule => rule.IsRequestAllowed(client, resource, currentTime, _clientRequests));
    }

    public void RecordRequest(string client, string resource)
    {
        var currentTime = DateTime.UtcNow;
        var key = (client, resource);
        if (!_clientRequests.ContainsKey(key))
        {
            _clientRequests[key] = new List<DateTime>();
        }
        _clientRequests[key].Add(currentTime);
    }

    public bool HandleRequest(string client, string resource)
    {
        if (IsRequestAllowed(client, resource))
        {
            RecordRequest(client, resource);
            return true;
        }
        return false;
    }
}
