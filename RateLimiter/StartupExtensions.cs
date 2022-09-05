using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Middleware;
using RateLimiter.RateLimiterProcessors;
using RateLimiter.Services;
using RateLimiter.Stores;
using RateLimiter.Stores.MemoryCache;
using RateLimiter.Stores.Repositories;
using System.Reflection;

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
            services.AddScoped<IRateLimiterProcessor, LastCallTimeSpanProcessor>();
            services.AddScoped<IRateLimiterProcessor, RequestRateProcessor>();
            // Services
            services.AddScoped<IRateLimiterService, RateLimiterService>();

            return services;
        }

        public static IApplicationBuilder UseClientRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimiterMiddleware>();
        }
    }
}
