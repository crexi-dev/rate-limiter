using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Engine;
using RateLimiter.Interfaces;
using RateLimiter.Rules;

namespace RateLimiter.Extensions
{
    public static class RateLimiterExtensions
    {
        /// <summary>
        /// Add the required Rate Limiter services to the D.I. framework
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection UseRateLimiter(this IServiceCollection services)
        {
            services.AddSingleton<IRateLimiterRulesEngine, SequentialRateLimiterRulesEngine>();
            services.AddSingleton<RequestsPerTimespanRule>();
            services.AddSingleton<TimeSinceLastRequestRule>();
            services.AddSingleton<FallbackRule>();
            
            return services;
        }

        /// <summary>
        /// Add the default Rules Engine options to the D.I. framework.
        /// To customize the rules, you should implement a custom IRateLimiterRulesEngineOptions service
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection UseDefaultRateLimiterOptions(this IServiceCollection services)
        {
            services.AddSingleton<IRateLimiterRulesEngineOptions, DefaultRateLimiterRulesEngineOptions>();

            return services;
        }
    }
}
