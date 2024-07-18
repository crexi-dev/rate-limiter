using System.Linq;

namespace RateLimiter;

/// <summary>
/// A request rate limiter. Can limit requests by clients to resources using configurable and extendable rules. <br/>
/// <see cref="RateLimiterBuilder{TClient, TResource}"/> can be used for easier construction of the <see cref="RateLimiter{TClient, TResource}"/> using fluent syntax.
/// </summary>
/// <typeparam name="TClient">The type of client (who is requesting a resource).</typeparam>
/// <typeparam name="TResource">The type of resource (what thing is being accessed).</typeparam>
/// <param name="rules">The <see cref="IRateLimitingRule{TClient, TResource}"/> that should be followed.</param>
public class RateLimiter<TClient, TResource>(DynamicDictionary<(TClient client, TResource resource), IRateLimitingRule<TClient, TResource>> rules) where TClient : notnull
{
    /// <summary>
    /// Given a client and a resource, determine if the request limit has been met. <br/>
    /// Note: If any one rule reaches a limit, then the limit as a whole has been reached.
    /// </summary>
    /// <param name="client">The client making the request.</param>
    /// <param name="resource">The resource being requested.</param>
    /// <returns><see cref="bool"/> indicating if the request limit has been met.</returns>
    public bool HasReachedLimit(TClient client, TResource resource)
    {
        return rules.Get((client, resource)).Any(rateLimitingRule => rateLimitingRule.HasReachedLimit(client, resource));
    }

    /// <summary>
    /// Registers a request from a client to a resource. Indicates a succesful resource access and should count towards the rate limit. <br/>
    /// Note: The request is registered to all rules applicable to the client / resource combination.
    /// </summary>
    /// <param name="client">The client making the request.</param>
    /// <param name="resource">The resource being requested.</param>
    public void RegisterRequest(TClient client, TResource resource)
    {
        foreach (var rule in rules.Get((client, resource)))
        {
            rule.RegisterRequest(client, resource);
        }
    }
}