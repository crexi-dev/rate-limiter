using RateLimiter.RateLimiter.Models;

namespace RateLimiter.RateLimiter
{
    public interface ILimiter
    {
        LimitResult CheckLimit(ClientRequest request);
    }
}
