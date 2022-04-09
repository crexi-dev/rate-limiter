
using RateLimiter.DataStorageSimulator;

namespace RateLimiter.Interfaces
{
    /// <summary>
    /// Public interface for the main service which performs Rate Limit validations per client per request
    /// </summary>
    public interface IRateLimiterService
    {
        bool IsRequestAllowed(Token token, AvailableResource reourse);
    }
}
