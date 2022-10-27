using RateLimiter.Models;
using System.Threading.Tasks;

namespace RateLimiter
{
    public interface IRateLimitService
    {
        Task<bool> ValidateRequest(ClientRequest clientRequest);
    }
}
