using System.Threading.Tasks;
using RateLimiter.Models;

namespace RateLimiter.Abstractions
{
    public interface IRateLimitRule
    {
        Task<bool> ValidateAsync(RequestContext context);
    }
}