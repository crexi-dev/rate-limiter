using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Contexts;
using RateLimiter.Middlewares;
using RateLimiter.Models;
using RateLimiter.RateLimitHandlers;
using RateLimiter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Extensions
{
    public static class RateLimiterServiceCollection
    {
        public static IServiceCollection ConfigureRateLimiter(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<RateLimitOptions>(Configuration.GetSection(RateLimitOptions.SectionName));
            services.AddScoped<ICacheService, CacheService>();
            
            services.ConfigureRateLimitHandlers();
            return services;
        }

        public static void ConfigureDbContext(this IServiceCollection services)
        { 
            services.AddScoped<RateDBContextBase>(); 
        }

        private static IServiceCollection ConfigureRateLimitHandlers(this IServiceCollection services)
        {
            //TO DO - IRateLimitHandler
            services.AddScoped<XRequestsPerTimespanHandler>();
            services.AddScoped<LastCallHandler>();
            services.AddScoped<USBasedTokensHandler>();
            services.AddScoped<EUBasedTokensHandler>();
            services.AddScoped<ProcessFactoryBase, ProcessFactory>();
            //services.AddScoped<RateLimitMiddleware<ProcessFactory>();
            return services;
        }
    }
}
