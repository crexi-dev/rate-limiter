namespace RateLimiter;

public interface IRateLimitingRule
{
    bool IsRequestAllowed(string clientId, string resource);
}