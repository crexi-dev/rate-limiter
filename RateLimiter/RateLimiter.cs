using RateLimiter.Interfaces;
using System.Collections.Generic;

namespace RateLimiter;
public class RateLimiter() : IRateLimiter
{
    private readonly Dictionary<string, List<IRule>> rulesByResource = [];

    public void AddRule(string resource, IRule rule)
    {
        if (rulesByResource.TryGetValue(resource, out List<IRule>? value))
        {
            value.Add(rule);
        }
        else
        {
            rulesByResource[resource] = [rule];
        }
    }
    public bool IsRequestAllowed(string resource, string token)
    {
        if (rulesByResource.TryGetValue(resource, out var rules))
        {
            foreach (var rule in rules)
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
