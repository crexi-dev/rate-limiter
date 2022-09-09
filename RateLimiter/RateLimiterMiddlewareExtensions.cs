using Microsoft.AspNetCore.Builder;

namespace RateLimiter
{
    /// <summary>
    /// Used to connect <see cref="RateLimiterMiddleware"> RateLimiterMiddleware </see> to an ASP Net Core Web API application.
    /// </summary>
    public static class RateLimiterMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimiter(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimiterMiddleware>();
        }
    }
}
