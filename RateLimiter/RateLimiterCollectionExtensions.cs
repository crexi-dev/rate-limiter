using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Attributes;
using RateLimiter.Contexts;
using RateLimiter.Contexts.Interfaces;
using RateLimiter.Middlewares;
using RateLimiter.Rules;
using RateLimiter.Rules.Interfaces;
using RateLimiter.Stores;
using RateLimiter.Stores.Interfaces;

namespace RateLimiter;

public static class RateLimiterCollectionExtensions
{
    public static IServiceCollection AddRateLimiter(
        this IServiceCollection services)
    {
        services.AddSingleton<IRateLimitStore, InMemoryStore>();
        services.AddScoped<IUserContext, UserContext>();

        #region Rules
        services.AddScoped<IRule<RegionLimitAttribute>, RegionRule>();
        services.AddScoped<IRule<LimitPerTimeRateLimitAttribute>, LimitPerTimeRule>();
        services.AddScoped<IRule<LastCallRateLimitAttribute>, LastCallRule>();
        services.AddScoped<IRule<RegionLastCallRateLimitAttribute>, RegionLastCallRateRule>();
        #endregion
        
        return services;
    }

    public static IApplicationBuilder UseRateLimiter(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<RateLimitMiddleware>();
        builder.UseMiddleware<RequestLoggerMiddleware>();
        
        return builder;
    }
}