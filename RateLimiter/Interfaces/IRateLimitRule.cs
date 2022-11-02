using RateLimiter.Models.Request;

namespace RateLimiter.Interfaces
{
    public interface IRateLimitRule
    {
        bool ValidateRequest(ClientRequest request);
    }
}
