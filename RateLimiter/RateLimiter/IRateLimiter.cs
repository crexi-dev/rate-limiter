namespace RateLimiter.RateLimiter;

public interface IRateLimiter
{
    void PassOrThrough(string api, string token);
}
