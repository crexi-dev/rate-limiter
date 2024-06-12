namespace RateLimiter.Interfaces;

public interface IRateLimiter
{
    void AddRule(string resource, IRule rule);
    bool IsRequestAllowed(string resource, string token);
}