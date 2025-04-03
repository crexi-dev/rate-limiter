using Microsoft.AspNetCore.Builder;
using RateLimiter.Http;

namespace RateLimiter.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRateLimiter(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimiterMiddleware>();
        }
    }
}