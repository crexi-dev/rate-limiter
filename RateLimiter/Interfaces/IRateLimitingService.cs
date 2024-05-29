using System.Threading.Tasks;
using RateLimiter.Models;

namespace RateLimiter.Interfaces
{
    /// <summary>
    /// Provides functionality for checking if a request is allowed based on configured rate limiting rules.
    /// </summary>
    public interface IRateLimitingService
    {
        /// <summary>
        /// Asynchronously checks if a request is allowed based on the configured rate limiting rules.
        /// </summary>
        /// <param name="resource">The name of the resource being accessed.</param>
        /// <param name="token">The access token identifying the client making the request.</param>
        /// <returns>A task result contains a <see cref="RateLimiterResult"/> indicating if the request is allowed and any associated errors.</returns>
        Task<RateLimiterResult> ValidateRequestAsync(string resource, string token);
    }
}