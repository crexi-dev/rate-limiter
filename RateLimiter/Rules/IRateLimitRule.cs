namespace RateLimiter.Rules;

public interface IRateLimitRule
{
    bool IsRequestAllowed(AccessToken token, string route);
}
