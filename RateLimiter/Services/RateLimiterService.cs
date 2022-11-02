using RateLimiter.Interfaces;
using RateLimiter.Models.Request;

namespace RateLimiter.Services
{
    public class RateLimiterService : IRateLimiterService
    {
        private readonly IRateLimitRule _rateLimitRule;
        public RateLimiterService(IRateLimitRule rateLimitRule)
        {
            _rateLimitRule = rateLimitRule;
        }

        public bool ValidateRequest(ClientRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
