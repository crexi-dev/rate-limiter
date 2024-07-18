namespace RateLimiter;
public interface IRateLimitingRule<TClient, TResource>
{
    bool HasReachedLimit(TClient client, TResource resource);
    void RegisterRequest(TClient client, TResource resource);
}
