namespace RateLimiter;

/// <summary>
/// Interface that should be implemented for rules that are used in <see cref="RateLimiter{TClient, TResource}"/> <br/>
/// Note: Rules can be configured to be one instance per client / resource combination, as a singleton for all client / resource combinations, or anywhere in between. <br/>
/// </summary>
/// <typeparam name="TClient">The type of client (who is requesting a resource).</typeparam>
/// <typeparam name="TResource">The type of resource (what thing is being accessed).</typeparam>
public interface IRateLimitingRule<TClient, TResource>
{
    /// <summary>
    /// Given a client and a resource, determine if the request limit has been met.
    /// </summary>
    /// <param name="client">The client making the request.</param>
    /// <param name="resource">The resource being requested.</param>
    /// <returns><see cref="bool"/> indicating if the request limit has been met.</returns>
    bool HasReachedLimit(TClient client, TResource resource);

    /// <summary>
    /// Registers a request from a client to a resource. Indicates a succesful resource access and should count towards the rate limit.
    /// </summary>
    /// <param name="client">The client making the request.</param>
    /// <param name="resource">The resource being requested.</param>
    void RegisterRequest(TClient client, TResource resource);
}
