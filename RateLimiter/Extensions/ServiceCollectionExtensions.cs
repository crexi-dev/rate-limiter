using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RateLimiter.Abstractions;
using RateLimiter.Configuration;
using RateLimiter.Http;
using RateLimiter.Storage;

namespace RateLimiter.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRateLimiter(
            this IServiceCollection services,
            Action<RateLimitOptions>? configureOptions = null)
        {
            services.TryAddSingleton<IConcurrentRequestTracker, InMemoryRequestTracker>();
            services.TryAddSingleton<IRequestContextFactory, DefaultRequestContextFactory>();
            
            var optionsBuilder = services.AddOptions<RateLimitOptions>();
            
            if (configureOptions != null)
            {
                optionsBuilder.Configure(configureOptions);
            }
            
            return services;
        }
    }
}