using RateLimiter.Models.Request;

namespace RateLimiter.Interfaces
{
    public interface IRateLimiterService
    {
        bool ValidateRequest(ClientRequest request);
    }
}
