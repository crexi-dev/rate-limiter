using System;
using System.Collections.Generic;

namespace RateLimiters;
public class RateLimiter
{
    private readonly Dictionary<string, RateLimitPolicy> _policies;

    public RateLimiter()
    {
        _policies = new Dictionary<string, RateLimitPolicy>();
    }

    public void AddPolicy(string resourceId, RateLimitPolicy policy)
    {
        _policies[resourceId] = policy;
    }

    public bool IsRequestAllowed(string clientId, string resourceId)
    {
        if (!_policies.ContainsKey(resourceId))
        {
            throw new ArgumentException($"No policy configured for resource: {resourceId}");
        }

        var policy = _policies[resourceId];
        return policy.IsRequestAllowed(clientId, resourceId);
    }

    public void RecordRequest(string clientId, string resourceId)
    {
        if (!_policies.ContainsKey(resourceId))
        {
            throw new ArgumentException($"No policy configured for resource: {resourceId}");
        }

        var policy = _policies[resourceId];
        policy.RecordRequest(clientId, resourceId);
    }
}
