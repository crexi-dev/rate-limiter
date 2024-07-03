namespace RateLimiter.Interfaces;

public interface IRateLimitingRule
{
    bool IsRequestAllowed(string token, string resource);
}