using AspNetCoreRateLimit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RateLimiter.API.RateLimiting.StartupConfiguration
{
    public static class RateLimitingInitialConfigurator
    {
        public static IServiceCollection ConfigureRateLimitingOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ClientRateLimitOptions>(configuration.GetSection("ClientRateLimiting"));
            services.Configure<ClientRateLimitPolicies>(configuration.GetSection("ClientRateLimitPolicies"));

            return services;
        }
    }
}
