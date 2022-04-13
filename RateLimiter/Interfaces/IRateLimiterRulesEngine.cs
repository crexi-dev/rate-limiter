using RateLimiter.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Interfaces
{
    /// <summary>
    /// Represents the core rules engine for the Rate Limiter
    /// </summary>
    public interface IRateLimiterRulesEngine
    {
        /// <summary>
        /// Call this method to determine whether a particular Request should be processed
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> CanProcessAsync(RequestInfo token, CancellationToken cancellationToken);
    }
}
