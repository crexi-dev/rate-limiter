using Microsoft.AspNetCore.Builder;
using RateLimiter.Models;

namespace RateLimiter
{
    public static class RateLimiterMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimiter(
            this IApplicationBuilder builder, RateLimitOptions options)
        {
            return builder.UseMiddleware<RateLimiterMiddleware>(options);
        }
    }
}
