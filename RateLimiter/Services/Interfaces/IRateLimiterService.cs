using System.Collections.Generic;
using System.Threading.Tasks;
using RateLimiter.Models.Enums;

namespace RateLimiter.Services.Interfaces
{
    public interface IRateLimiterService
    {
        public Task ValidateRateLimitsAsync(string token, List<RateLimiterType> rateLimiterTypes);
    }
}
