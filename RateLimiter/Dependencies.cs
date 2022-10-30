using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Caches;
using RateLimiter.Rules;
using RateLimiter.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public static class Dependencies
    {
        public static IServiceCollection ConfigureRateLimiter(this IServiceCollection services)
        {
            services.AddSingleton<IClientRequestsCache, ClientRequestsCache>();

            services.AddSingleton<IBaseRule, NumberRequestsPerTimeRule>();
            services.AddSingleton<IBaseRule, TimeBetweenTwoRequestsRule>();

            services.AddScoped<IRateLimitService, RateLimitService>();

            return services;
        }
    }
}
