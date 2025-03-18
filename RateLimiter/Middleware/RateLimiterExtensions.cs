using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Models;
using RateLimiter.Services;

namespace RateLimiter.Middleware;

/// <summary>
/// Provides extension methods for configuring and using the rate limiter middleware.
/// </summary>
public static class RateLimiterExtensions
{
    /// <summary>
    /// Adds the rate limiter service to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add the rate limiter to.</param>
    /// <param name="options">A configuration action to set up rate limiter options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRateLimiter(this IServiceCollection services, Action<RateLimiterOptions> options)
    {
        RateLimiterOptions rateLimiterOptions = new();
        options(rateLimiterOptions);

        services.AddSingleton<IRateLimiterService>(provider => new RateLimiterService(rateLimiterOptions.Rules));
        return services;
    }

    /// <summary>
    /// Adds the rate limiter middleware to the application's request pipeline.
    /// </summary>
    /// <param name="app">The application builder to configure the middleware.</param>
    /// <returns>The updated application builder.</returns>
    public static IApplicationBuilder UseRateLimiter(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<RateLimiterOptions>();
        var rateLimiter = app.ApplicationServices.GetRequiredService<IRateLimiterService>();

        return app.UseMiddleware<RateLimiterMiddleware>(rateLimiter, options);
    }
}