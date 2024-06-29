using System.Collections.Generic;
using System.Linq;

public class RateLimitPolicy
{
    private readonly List<IRateLimitRule> _rules;

    public RateLimitPolicy(params IRateLimitRule[] rules)
    {
        _rules = rules.ToList();
    }

    public bool IsRequestAllowed(string clientId, string resourceId)
    {
        return _rules.All(rule => rule.IsRequestAllowed(clientId, resourceId));
    }

    public void RecordRequest(string clientId, string resourceId)
    {
        foreach (var rule in _rules)
        {
            rule.RecordRequest(clientId, resourceId);
        }
    }
}
