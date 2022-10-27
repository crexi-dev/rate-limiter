using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.InMemoryStore;
using RateLimiter.Middleware;

namespace RateLimiter.Extensions
{
    // StartUp Extension class to make middleware implementation easier
    public static class StartUpExtensions
    {
        public static IServiceCollection AddRateLimiter(this IServiceCollection services)
        {
            services.AddSingleton<IRateLimitService, RateLimitService>();
            services.AddSingleton<ICacheStore, DistributedCache>();
            return services;
        }

        public static IApplicationBuilder UseRateLimiter(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitMiddleware>();
        }
    }
}
