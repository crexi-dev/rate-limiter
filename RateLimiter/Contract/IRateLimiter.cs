using System.Threading.Tasks;

namespace RateLimiter.Contract
{
    public interface IRateLimiter
    {
        Task<bool> RateLimitExceeded(Request request);
    }
}
