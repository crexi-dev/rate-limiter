namespace RateLimiter.Rules
{
    public interface IRateLimitRule
    {
        bool IsRequestAllowed(string resource, string token);
    }
}
