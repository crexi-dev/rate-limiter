using AspNetCoreRateLimit;
using Microsoft.Extensions.DependencyInjection;

namespace RateLimiter.Client.Configuration.Initial
{
    /// <summary>
    /// Initial configurator
    /// </summary>
    internal static class RateLimitingLibraryInitialConfigurator
    {
        /// <summary>
        /// Configures needed services adding for proper working of rate limiting library
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IServiceCollection ConfigureRateLimitingLibrary(this IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, ExtendedClientRateLimitConfiguration>();

            return services;
        }
    }
}
