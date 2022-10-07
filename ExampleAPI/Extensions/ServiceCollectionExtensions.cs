using API.Middlewares;
using RateLimiter.Configuration.Options;
using RateLimiter.Services;

namespace API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<LimiterStore>()
                .AddTransient<RequestLimiterMiddleware>();
            
            serviceCollection
                .AddOptions<LimiterOptions>()
                .Configure<IConfiguration>((options, configuration) =>
                {
                    configuration.GetSection(nameof(LimiterOptions)).Bind(options);
                });
            
            return serviceCollection;
        }
    }
}