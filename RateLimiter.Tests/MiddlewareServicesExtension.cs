using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Cache;
using RateLimiter.LocationService;
using RateLimiter.RulesEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    public static class MiddlewareExtensions
    {
        public static IServiceCollection AddMyServices(this IServiceCollection services)
        {
            services.UseRateLimiter();
            services.AddSingleton<ILocationService, LocationService.LocationService>();
            services.AddSingleton<IStorageService, InMemoryService>();
            services.AddSingleton(typeof(IConfiguration), Configuration.GetConfiguration);

            return services;
        }
    }    
}
