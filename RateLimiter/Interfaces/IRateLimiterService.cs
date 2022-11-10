using RateLimiter.Enums;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Interfaces
{
    public interface IRateLimiterService
    {
        Task<bool> ValidateRequestAsync(string clientId, DateTime requestDate, string endpoint, Location location);
    }
}
