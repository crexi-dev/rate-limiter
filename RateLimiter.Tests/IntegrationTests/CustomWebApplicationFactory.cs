using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RateLimiter.Common.Abstractions;
using RateLimiter.Common.Abstractions.Counters;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Core.Configuration;
using RateLimiter.Core.Services;
using RateLimiter.Core.Services.KeyBuilders;
using RateLimiter.Infrastructure.Counters;

namespace RateLimiter.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        // Add logging for tests
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddDebug();
        });

        builder.ConfigureServices(services =>
        {
            // Replace rate limit counter with a new instance to ensure clean state between tests
            RemoveAllServiceRegistrationsOf<IRateLimitCounter>(services);
            RemoveAllServiceRegistrationsOf<IMemoryCache>(services);
            RemoveAllServiceRegistrationsOf<IRateLimiterService>(services);
            RemoveAllServiceRegistrationsOf<IRateLimitRuleProvider>(services);
            RemoveAllServiceRegistrationsOf<IKeyBuilder>(services);
            RemoveAllServiceRegistrationsOf<IRateLimitClientIdentifierProvider>(services);

            // Add a memory cache for testing
            services.AddMemoryCache();
            
            // Configure rate limit options
            services.Configure<RateLimitOptions>(options =>
            {
                options.EnableRateLimiting = true;
                options.IncludeHeaders = true;
                options.HeaderPrefix = "X-RateLimit";
                options.StatusCode = 429;
                options.ClientIdHeaderName = "X-ClientId";
                options.RegionHeaderName = "X-Region";
            });
            
            // Add key builder
            services.AddSingleton<IKeyBuilder, ResourceKeyBuilder>();
            
            // Add a memory counter with a singleton lifetime for testing
            services.AddSingleton<IRateLimitCounter, MemoryRateLimitCounter>();
            
            // Add client identifier provider
            services.AddSingleton<IRateLimitClientIdentifierProvider, DefaultClientIdentifierProvider>();
            
            // Add rule provider
            services.AddSingleton<IRateLimitRuleProvider, AttributeBasedRuleProvider>();
            
            // Ensure services are registered in the right order
            services.AddSingleton<IRateLimiterService, RateLimiterService>();
        });
    }

    public new WebApplicationFactory<TProgram> WithWebHostBuilder(Action<IWebHostBuilder> configure)
    {
        return base.WithWebHostBuilder(builder => 
        {
            configure(builder);
                
            // Ensure service registration
            builder.ConfigureServices(services =>
            {
                var serviceRegistrations = services.Where(s => s.ServiceType == typeof(IRateLimiterService)).ToList();
                if (!serviceRegistrations.Any() || serviceRegistrations.Any(s => s.Lifetime != ServiceLifetime.Singleton))
                {
                    // Remove any existing non-singleton registrations
                    foreach (var reg in serviceRegistrations)
                    {
                        services.Remove(reg);
                    }
                    
                    // Add singleton service
                    services.AddSingleton<IRateLimiterService, RateLimiterService>();
                }
            });
        });
    }
    
    // Helper method to remove all services of a specific type
    private static void RemoveAllServiceRegistrationsOf<T>(IServiceCollection services)
    {
        var serviceDescriptors = services.Where(descriptor => descriptor.ServiceType == typeof(T)).ToList();
        foreach (var serviceDescriptor in serviceDescriptors)
        {
            services.Remove(serviceDescriptor);
        }
    }
}