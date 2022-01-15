using System.Threading.Tasks;

namespace RateLimiter.RateLimiter.Services
{
    public interface IRateLimitService
    {
        Task<bool> ValidateAsync(string accessToken, string resourceName);
    }
}
