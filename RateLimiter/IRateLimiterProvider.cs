using RateLimiter.Models;

namespace RateLimiter
{
    internal interface IRateLimiterProvider
    {
        public RateLimiter GetRateLimiter(Request request);
    }
}