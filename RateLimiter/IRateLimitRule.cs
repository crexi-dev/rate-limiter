namespace RateLimiter;

public interface IRateLimitRule
{
    bool IsRequestAllowed(string token);
}