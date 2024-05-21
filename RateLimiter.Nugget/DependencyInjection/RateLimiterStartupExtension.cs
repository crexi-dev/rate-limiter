using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Nugget.DataStore;
using RateLimiter.Nugget.Interfaces;
using RateLimiter.Nugget.Services;

namespace RateLimiter.Nugget.DependencyInjection
{
    public static class RateLimiterStartupExtension
    {
        public static IServiceCollection AddRateLimiting<T>(this IServiceCollection services)
        where T : IRateLimitRule<T>
        {
            //TODO do some injections here
            services.AddTransient<RateLimiter<T>>();
            services.AddSingleton(typeof(ICacheServiceBase<>), typeof(CacheServiceBase<>));
            services.AddSingleton<ICacheServiceBase, CacheServiceBase>();

            return services;
        }

        // public static IServiceCollection AddRateLimiting(this IServiceCollection services, List<IRateLimitRule> rules)
        // {
        //     //TODO do some injections here
        //     services.AddTransient<RateLimiter>();
        //     services.AddSingleton(typeof(ICacheServiceBase<>), typeof(CacheServiceBase<>));
        //     services.AddSingleton<ICacheServiceBase, CacheServiceBase>();
        //
        //     foreach (var rule in rules)
        //     {
        //         SharedRoutes.AddRoutes<T>(rule.GetRoutes());
        //     }
        //
        //     return services;
        // }

        public static IServiceCollection AddRateLimitingRules<T>(this IServiceCollection services, IRateLimitRule<T> rule)
            where T : IRateLimitRule<T>
        {
            SharedRoutes.AddRoutes<T>(rule.GetRoutes());
            return services;
        }
    }
}
