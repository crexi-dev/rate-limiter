using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Services;
using RateLimiter.Services.Interfaces;

namespace RateLimiter;

public static class RateLimiterServiceCollectionExtensions
{
    /// <summary>
    /// Adds an implementation of IRateLimitStorageService with memory cache
    /// </summary>
    public static IServiceCollection AddRateLimitingServiceWithMemoryCache(this IServiceCollection services)
    {
        return services
            .AddRateLimitingServiceCore()
            .AddMemoryCache()
            .AddSingleton<IRateLimitStorageService, MemoryCacheRateLimitStorageService>();
    }

    /// <summary>
    /// Adds an implementation of IRateLimitStorageService with distributed cache
    /// </summary>
    public static IServiceCollection AddRateLimitingServiceWithDistributedCache(this IServiceCollection services)
    {
        return services
            .AddRateLimitingServiceCore()
            .AddSingleton<IRateLimitStorageService, DistributedCacheRateLimitStorageService>();
    }

    /// <summary>
    /// Adds an implementation of IRateLimitStorageService without IRateLimitStorageService implementation
    /// </summary>
    /// <remarks>Additional needs to add implementation of IRateLimitStorageService</remarks>
    public static IServiceCollection AddRateLimitingServiceCore(this IServiceCollection services)
    {
        return services
            .AddSingleton<IRateLimitingService, RateLimitingService>()
            .AddSingleton<IDateTimeProvider, DefaultDateTimeProvider>();
    }
}
