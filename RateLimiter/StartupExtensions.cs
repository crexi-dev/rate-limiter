using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Middleware;
using RateLimiter.RateLimiterProcessors;
using RateLimiter.Services;
using RateLimiter.Stores;
using RateLimiter.Stores.MemoryCache;
using RateLimiter.Stores.Repositories;

namespace RateLimiter
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddInMemoryRateLimiting(this IServiceCollection services)
        {
            // Stores
            services.AddSingleton<ICacheProvider, MemoryCacheStore>(); // switch via ClientRequestRepository or MemoryCacheStore
            services.AddSingleton<IClientRequestRepository, ClientRequestRepository>();
            services.AddSingleton<IMemoryCacheStore, MemoryCacheStore>();
            // Processors
            services.AddSingleton<IRateLimiterProcessor, LastCallTimeSpanProcessor>();
            services.AddSingleton<IRateLimiterProcessor, RequestRateProcessor>();
            // Services
            services.AddSingleton<IRateLimiterService, RateLimiterService>();

            return services;
        }

        public static IApplicationBuilder UseClientRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimiterMiddleware>();
        }
    }
}
