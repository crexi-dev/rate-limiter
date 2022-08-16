using RateLimiter.Models;

namespace RateLimiter
{
    public interface IRateLimiter
    {
        RateLimiterResult Execute(RateLimiterContext context);
    }
}
