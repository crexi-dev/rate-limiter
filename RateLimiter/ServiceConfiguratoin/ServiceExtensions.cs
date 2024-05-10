using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Executors;
using RateLimiter.Models;
using RateLimiter.Processor;
using RateLimiter.Profile;
using RateLimiter.Reader;
using System.Collections.Generic;

namespace RateLimiter.ServiceConfiguratoin
{
    public static class ServiceExtensions
    {
        public static void RegisterRateLimiterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IRateLimitProcessor, RateLimitProcessor>();
            services.AddSingleton<IRuleReader, RuleReader>();
            services.AddSingleton<IRuleExecutor, RuleExecutor>();
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddMemoryCache();
            services.Configure<List<RuleConfigurationModel>>(options => configuration.GetSection("Rules").Bind(options));
        }
    }
}
