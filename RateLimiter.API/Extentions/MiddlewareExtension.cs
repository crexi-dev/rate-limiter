using RateLimiter.API.Middleware;

namespace RateLimiter.API.Extentions
{
    public static class MiddlewareExtension
    {
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitingMiddleware>();
        }
    }
}
