using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Rules;
using RateLimiter.Rules.Interfaces;
using RateLimiter.Storage;

namespace RateLimiter.Extensions;

public static class StartupExtension
{
    public static IServiceCollection ConfigureRateLimiter(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddMemoryCache();
        services.AddSingleton<IStorage<RateLimitEntry>, InMemoryStorage<RateLimitEntry>>();
        services.AddSingleton<IRateLimiterRule, CertainTimeSpanSinceLastCallRule>();
        services.AddSingleton<IRateLimiterRule, RequestsPerTimeSpanRule>();
        services.Configure<RateLimitOptions>(configuration.GetSection(nameof(RateLimitOptions)));

        return services;
    }

    public static IApplicationBuilder UseRateLimiter(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimiterMiddleware>();
    }
}