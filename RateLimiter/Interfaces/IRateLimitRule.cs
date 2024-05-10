namespace RateLimiter.Interfaces
{
    // Interface defining the structure for rate limiting rules
    public interface IRateLimitRule
    {
        bool IsRequestAllowed(string token, string resource);
    }
}
