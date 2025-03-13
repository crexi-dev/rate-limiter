namespace RateLimiter.Rules;

public interface IRateLimitRule
{
    bool AllowRequest(string clientId);
}