using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace RateLimiter.Configuration
{
    public static class Extensions
    {
        public static IServiceCollection AddRateLimiter(this IServiceCollection services, Action<RateLimiterOptions> configure)
        {
            if (configure == null)
            {
                throw new InvalidOperationException("Rate limiter configuration is missing");
            }

            var options = new RateLimiterOptions();

            configure(options);

            services.AddSingleton(options);

            services.AddSingleton<IRateLimiter, RateLimiter>();

            return services;
        }
    }
}
