using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Repositories;

namespace RateLimiter
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddInMemoryRateLimiting(this IServiceCollection services)
        {
            services.AddSingleton<IClientRequestRepository, ClientRequestRepository>();
            //services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            //services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            //services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            return services;
        }
    }
}
