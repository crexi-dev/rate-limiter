using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.Client.Configuration
{
    /// <summary>
    /// Contributes current client information for rate limiting library
    /// </summary>
    public class ExtendedClientResolveContributor : IClientResolveContributor
    {
        /// <summary>
        /// Resolves extraction and contributing client data for Rate limiting library
        /// </summary>
        public Task<string> ResolveClientAsync(HttpContext httpContext)
        {
            // Note- we are not using real-life authorization example and environment here, just header-extraction
            if (httpContext.Request.Headers.TryGetValue("X-ClientAccessToken", out var clientAccessor))
            {
                return Task.FromResult(clientAccessor.First());
            }
            return Task.FromResult(string.Empty);
        }
    }
}
