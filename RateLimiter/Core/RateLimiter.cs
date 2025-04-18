using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Core.Rules;

namespace RateLimiter.Core
{
    public class RateLimiter
{
    private readonly ConcurrentDictionary<string, IRateLimitRule> _resourceRules = new();

    // configure a rule for a resource
    public void ConfigureResource(string resourceId, IRateLimitRule rule)
    {
        _resourceRules[resourceId] = rule;
    }

    // check if a request should be allowed
    public bool IsRequestAllowed(string clientToken, string resourceId)
    {
        // if no rule for this resource, allow the request
        if (!_resourceRules.TryGetValue(resourceId, out var rule)) {
            return true;
        }

        // check the rule
        return rule.IsAllowed(clientToken, resourceId);
    }
}
}