using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Stores;
using RateLimiter.Stores.MemoryCache;
using RateLimiter.Stores.Repositories;

namespace RateLimiter
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddInMemoryRateLimiting(this IServiceCollection services)
        {
            // https://devkimchi.com/2020/07/01/5-ways-injecting-multiple-instances-of-same-interface-on-aspnet-core/
            services.AddScoped<ICacheProvider, MemoryCacheStore>(); // switch via ClientRequestRepository or MemoryCacheStore
            services.AddSingleton<IClientRequestRepository, ClientRequestRepository>();
            services.AddScoped<IMemoryCacheStore, MemoryCacheStore>();
            //services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            //services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            //services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            return services;
        }
    }
}
