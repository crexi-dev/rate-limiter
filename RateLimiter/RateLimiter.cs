using System.Linq;

namespace RateLimiter;

public class RateLimiter<TClient, TResource>(DynamicDictionary<(TClient client, TResource resource), IRateLimitingRule<TClient, TResource>> rules) where TClient : notnull
{
    public bool HasReachedLimit(TClient client, TResource resource)
    {
        return rules.Get((client, resource)).Any(rateLimitingRule => rateLimitingRule.HasReachedLimit(client, resource));
    }

    public void RegisterRequest(TClient client, TResource resource)
    {
        foreach (var rule in rules.Get((client, resource)))
        {
            rule.RegisterRequest(client, resource);
        }
    }
}