using Microsoft.Extensions.DependencyInjection;
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
            services.AddScoped<ICacheProvider, MemoryCacheStore>(); // switch via ClientRequestRepository or MemoryCacheStore
            services.AddSingleton<IClientRequestRepository, ClientRequestRepository>();
            services.AddScoped<IMemoryCacheStore, MemoryCacheStore>();
            // Processors
            services.AddScoped<IRateLimiterProcessor, LastCallTimeSpanProcessor>();
            services.AddScoped<IRateLimiterProcessor, RequestRateProcessor>();
            // Services
            services.AddScoped<IRateLimiterService, RateLimiterService>();

            return services;
        }
    }
}
