using RateLimiter.Services;

namespace RateLimiter.Middleware
{
    public class RateLimiterMiddleware
    {
        private readonly IRateLimiterService rateLimiterService;

        public RateLimiterMiddleware(IRateLimiterService rateLimiterService)
        {
            this.rateLimiterService = rateLimiterService;
        }
    }
}
