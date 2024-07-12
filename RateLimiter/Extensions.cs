using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Implementations;
using RateLimiter.Interfaces;

namespace RateLimiter;

public static class Extensions
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        var providers = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(IRuleContextProvider).IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false });
        
        foreach (var provider in providers)
        {
            services.AddTransient(provider);
        }

        services.AddHttpContextAccessor();
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        // this can be replaced in DI if needed
        services.AddTransient<IStore, InMemoryStore>();
        services.AddTransient<EmptyContextProvider>();

        return services;
    }

    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RateLimitingMiddleware>();
    }
}