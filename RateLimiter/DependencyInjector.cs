using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Client.Configuration.Initial;

namespace RateLimiter
{
    public static class DependencyInjector
    {
        public static IServiceCollection InjectRateLimiter(this IServiceCollection services)
        {
            // For common injections and configurations ...

            // Configuring Rate limiting
            services.ConfigureRateLimitingLibrary();

            return services;
        }
    }
}
