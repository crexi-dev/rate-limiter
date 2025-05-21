using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Core.Configuration;
using RateLimiter.Core.Services;

namespace RateLimiter.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for setting up hybrid rate limiting
/// </summary>
public static class HybridRateLimiterExtensions
{
    /// <summary>
    /// Adds hybrid rate limiting that supports both configuration and attribute-based rules
    /// </summary>
    public static IServiceCollection AddHybridRateLimiting(
        this IServiceCollection services, 
        Action<RateLimitOptions>? configureRateLimit = null,
        Action<EnhancedRateLimitConfiguration>? configureRules = null)
    {
        // Add base rate limiting services first
        services.AddRateLimiting(configureRateLimit);
        
        // Configure enhanced rules
        if (configureRules != null)
        {
            services.Configure(configureRules);
        }
        
        // Register configuration-based rule provider
        services.TryAddSingleton<ConfigurationRuleProvider>();
        
        // Register attribute-based rule provider (keep existing)
        services.TryAddSingleton<AttributeBasedRuleProvider>();
        
        // Replace the main rule provider with hybrid implementation
        services.RemoveAll<IRateLimitRuleProvider>();
        services.AddSingleton<IRateLimitRuleProvider>(serviceProvider =>
        {
            var configProvider = serviceProvider.GetService<ConfigurationRuleProvider>();
            var attributeProvider = serviceProvider.GetService<AttributeBasedRuleProvider>();
            var config = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<EnhancedRateLimitConfiguration>>();
            var logger = serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<HybridRuleProvider>>();
            
            return new HybridRuleProvider(config, logger, configProvider, attributeProvider);
        });
        
        return services;
    }
    
    /// <summary>
    /// Adds hybrid rate limiting with enhanced authentication support using existing infrastructure
    /// </summary>
    public static IServiceCollection AddFullHybridRateLimiting(
        this IServiceCollection services, 
        Action<RateLimitOptions>? configureRateLimit = null,
        Action<EnhancedRateLimitConfiguration>? configureRules = null,
        Action<JwtAuthenticationOptions>? configureJwt = null,
        Action<GeoIPOptions>? configureGeoIP = null)
    {
        // Add hybrid rate limiting first
        services.AddHybridRateLimiting(configureRateLimit, configureRules);
        
        // Use existing enhanced authentication setup if JWT is configured
        if (configureJwt != null)
        {
            // Leverage the existing enhanced rate limiting setup
            services.AddFullEnhancedRateLimiting(configureRateLimit, configureJwt, configureGeoIP);
            
            // Override the rule provider with our hybrid version after enhanced setup
            services.RemoveAll<IRateLimitRuleProvider>();
            services.AddSingleton<IRateLimitRuleProvider>(serviceProvider =>
            {
                var configProvider = serviceProvider.GetService<ConfigurationRuleProvider>();
                var attributeProvider = serviceProvider.GetService<AttributeBasedRuleProvider>();
                var config = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<EnhancedRateLimitConfiguration>>();
                var logger = serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<HybridRuleProvider>>();
                
                return new HybridRuleProvider(config, logger, configProvider, attributeProvider);
            });
        }
        
        return services;
    }
}
