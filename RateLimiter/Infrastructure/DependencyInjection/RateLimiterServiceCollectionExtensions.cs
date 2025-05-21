using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RateLimiter.Common.Abstractions;
using RateLimiter.Common.Abstractions.Counters;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Core.Configuration;
using RateLimiter.Core.Services;
using RateLimiter.Core.Services.KeyBuilders;
using RateLimiter.Infrastructure.Counters;
using RateLimiter.Infrastructure.Services;
using StackExchange.Redis;

namespace RateLimiter.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for setting up rate limiting services.
/// Uses proper DI patterns to avoid duplicate registrations.
/// </summary>
public static class RateLimiterServiceCollectionExtensions
{
    /// <summary>
    /// Adds basic rate limiting services to the service collection.
    /// This is the foundation that other enhanced methods build upon.
    /// 
    /// REGISTRATION CONDITIONS:
    /// - Called directly for basic rate limiting (no auth/GeoIP)
    /// - Called internally by enhanced methods as foundation
    /// </summary>
    public static IServiceCollection AddRateLimiting(this IServiceCollection services, Action<RateLimitOptions>? configureOptions = null)
    {
        // Configure options
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        
        // Add core services (only if not already registered)
        services.TryAddSingleton<IKeyBuilder, DefaultKeyBuilder>();
        services.TryAddSingleton<IRateLimitClientIdentifierProvider, DefaultClientIdentifierProvider>();
        services.TryAddSingleton<IRateLimiterService, RateLimiterService>();
        services.TryAddSingleton<IRateLimitRuleProvider, AttributeBasedRuleProvider>();
        
        // Add memory caching and counter (only if not already registered)
        services.AddMemoryCache();
        services.TryAddSingleton<IRateLimitCounter, MemoryRateLimitCounter>();
        
        return services;
    }
    
    /// <summary>
    /// Adds enhanced rate limiting with secure authentication and GeoIP support.
    /// 
    /// REGISTRATION CONDITIONS:
    /// - Called when JWT or GeoIP configuration is present
    /// - Builds upon AddRateLimiting but replaces key services
    /// - Uses JWT authentication only
    /// </summary>
    public static IServiceCollection AddEnhancedRateLimiting(
        this IServiceCollection services, 
        Action<RateLimitOptions>? configureRateLimit = null,
        Action<JwtAuthenticationOptions>? configureJwt = null,
        Action<GeoIPOptions>? configureGeoIP = null)
    {
        // Add base rate limiting services first
        services.AddRateLimiting(configureRateLimit);
        
        // Configure enhanced authentication options
        if (configureJwt != null)
        {
            services.Configure(configureJwt);
        }
        
        // Configure GeoIP options
        if (configureGeoIP != null)
        {
            services.Configure(configureGeoIP);
        }
        
        // Add enhanced authentication service (JWT only)
        services.TryAddSingleton<IAuthenticationService, JwtAuthenticationService>();
        
        // Add GeoIP service
        services.TryAddSingleton<IGeoIPService, MaxMindGeoIPService>();
        
        // IMPORTANT: Replace the default client identifier provider with secure version
        services.RemoveAll<IRateLimitClientIdentifierProvider>();
        services.AddSingleton<IRateLimitClientIdentifierProvider, SecureClientIdentifierProvider>();
        
        return services;
    }
    
    /// <summary>
    /// Adds enhanced rate limiting with both JWT and API key authentication.
    /// 
    /// REGISTRATION CONDITIONS:
    /// - Called from Program.cs when useEnhancedRateLimiting is true
    /// - Provides both JWT and API key authentication
    /// - Most feature-complete configuration
    /// </summary>
    public static IServiceCollection AddFullEnhancedRateLimiting(
        this IServiceCollection services, 
        Action<RateLimitOptions>? configureRateLimit = null,
        Action<JwtAuthenticationOptions>? configureJwt = null,
        Action<GeoIPOptions>? configureGeoIP = null)
    {
        // Add base rate limiting services first
        services.AddRateLimiting(configureRateLimit);
        
        // Configure enhanced options
        if (configureJwt != null)
        {
            services.Configure(configureJwt);
        }
        
        if (configureGeoIP != null)
        {
            services.Configure(configureGeoIP);
        }
        
        // Add individual authentication services
        services.TryAddSingleton<JwtAuthenticationService>();
        services.TryAddSingleton<SimpleApiKeyService>();
        
        // Create collection of authentication services for composite
        services.TryAddSingleton<IEnumerable<IAuthenticationService>>(sp => new List<IAuthenticationService>
        {
            sp.GetRequiredService<JwtAuthenticationService>(),
            sp.GetRequiredService<SimpleApiKeyService>()
        });
        
        // IMPORTANT: Replace any existing authentication service with composite
        services.RemoveAll<IAuthenticationService>();
        services.AddSingleton<IAuthenticationService, CompositeAuthenticationService>();
        
        // Add GeoIP service
        services.TryAddSingleton<IGeoIPService, MaxMindGeoIPService>();
        
        // IMPORTANT: Replace the default client identifier provider with secure version
        services.RemoveAll<IRateLimitClientIdentifierProvider>();
        services.AddSingleton<IRateLimitClientIdentifierProvider, SecureClientIdentifierProvider>();
        
        return services;
    }
    
    /// <summary>
    /// Adds Redis rate limiting counter for distributed scenarios.
    /// 
    /// REGISTRATION CONDITIONS:
    /// - Called when Redis connection string is configured
    /// - Replaces memory counter with Redis for scalability
    /// - Can be used with any of the above configurations
    /// </summary>
    public static IServiceCollection AddRedisRateLimiting(this IServiceCollection services, string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Redis connection string cannot be null or empty", nameof(connectionString));
        }
        
        // Add Redis ConnectionMultiplexer
        services.TryAddSingleton<IConnectionMultiplexer>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<RedisRateLimitCounter>>();
            
            try
            {
                var options = ConfigurationOptions.Parse(connectionString);
                options.AbortOnConnectFail = false;
                
                var redis = ConnectionMultiplexer.Connect(options);
                
                redis.ConnectionFailed += (sender, e) =>
                {
                    logger.LogError("Redis connection failed: {EndPoint}, {FailureType}", e.EndPoint, e.FailureType);
                };
                
                redis.ConnectionRestored += (sender, e) =>
                {
                    logger.LogInformation("Redis connection restored: {EndPoint}", e.EndPoint);
                };
                
                redis.ErrorMessage += (sender, e) =>
                {
                    logger.LogError("Redis error: {Message}", e.Message);
                };
                
                return redis;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error connecting to Redis");
                throw;
            }
        });
        
        // IMPORTANT: Replace memory counter with Redis counter
        services.RemoveAll<IRateLimitCounter>();
        services.AddSingleton<IRateLimitCounter, RedisRateLimitCounter>();
        
        return services;
    }
    
    /// <summary>
    /// Adds a resource-based key builder.
    /// 
    /// REGISTRATION CONDITIONS:
    /// - Called from Program.cs for all configurations
    /// - Replaces default key builder with resource-aware version
    /// </summary>
    public static IServiceCollection AddResourceBasedKeyBuilder(this IServiceCollection services, bool includeHttpMethod = true, bool normalizeResourceNames = true)
    {
        // IMPORTANT: Replace default key builder
        services.RemoveAll<IKeyBuilder>();
        services.AddSingleton<IKeyBuilder>(sp => new ResourceKeyBuilder(includeHttpMethod, normalizeResourceNames));
        return services;
    }
}
