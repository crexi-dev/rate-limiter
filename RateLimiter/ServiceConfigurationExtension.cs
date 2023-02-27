using System;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceConfigurationExtension
{
    public static IServiceCollection AddRateLimiter(this IServiceCollection services, Action<RateLimiterOptions> configure)
    {
        var options = new RateLimiterOptions();
        configure(options);
        services.AddSingleton(options);
        services.AddSingleton<IRequestCacheService, RequestCacheService>();
        services.AddTransient<IRateLimiterService, RateLimiterService>();
        return services;
    }
}