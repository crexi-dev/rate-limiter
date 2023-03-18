using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Services;

namespace RateLimiter.DI;

public static class RateLimiterLogicRegistration
{
    public static IServiceCollection AddRateLimiterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRulesService, RulesService>();
        return services;
    }
}