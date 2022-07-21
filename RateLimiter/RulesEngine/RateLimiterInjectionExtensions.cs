using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Cache;
using RateLimiter.RulesEngine.Interfaces;

namespace RateLimiter.RulesEngine
{
    public static class RateLimiterInjectionExtensions
    {
        public static IServiceCollection UseRateLimiter(this IServiceCollection services)
        {
            services.AddSingleton<IRuleEngine, RuleEngine>();
            services.AddSingleton<IRateLimiterRule, RequestsPerTimeRule>();
            services.AddSingleton<IRateLimiterRule, TimeRule>();

            return services;
        }
    }
}
