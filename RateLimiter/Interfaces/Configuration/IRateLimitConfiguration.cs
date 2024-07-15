using RateLimiter.Models;

namespace RateLimiter.Interfaces.Configuration
{
    /// <summary>
    /// Describes configuration information used by the rate-limiting endgine
    /// </summary>
    /// <remarks>appsettings configuration target for rate limiting</remarks>
    public interface IRateLimitConfiguration
    {
        /// <summary>
        /// Stores the endpoints for which rate-limitig applies
        /// </summary>
        Endpoint[] Endpoints { get; init; }
    }
}