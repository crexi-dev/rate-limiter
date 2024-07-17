namespace RateLimiter;
public interface IRateLimitingRule<TClient, TResource>
{
    bool HasExceededLimit(TClient client, TResource resource);
    void RegisterRequest(TClient client, TResource resource);
}
