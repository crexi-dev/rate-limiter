using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Core.Configuration;
using RateLimiter.Core.Services;

namespace RateLimiter.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for setting up enhanced hybrid rate limiting with proper precedence
/// </summary>
public static class EnhancedHybridRateLimiterExtensions
{
    /// <summary>
    /// Adds enhanced hybrid rate limiting with proper configuration precedence
    /// </summary>
    public static IServiceCollection AddEnhancedHybridRateLimiting(
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
        
        // Replace the main rule provider with enhanced hybrid implementation
        services.RemoveAll<IRateLimitRuleProvider>();
        services.AddSingleton<IRateLimitRuleProvider>(serviceProvider =>
        {
            var configProvider = serviceProvider.GetService<ConfigurationRuleProvider>();
            var attributeProvider = serviceProvider.GetService<AttributeBasedRuleProvider>();
            var config = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<EnhancedRateLimitConfiguration>>();
            var logger = serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<EnhancedHybridRuleProvider>>();
            
            return new EnhancedHybridRuleProvider(config, logger, configProvider, attributeProvider);
        });
        
        // Replace the rate limiter service with enhanced version
        services.RemoveAll<IRateLimiterService>();
        services.AddSingleton<IRateLimiterService, EnhancedRateLimiterService>();
        
        return services;
    }
}
