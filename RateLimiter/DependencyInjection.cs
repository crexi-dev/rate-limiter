using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Middleware;
using RateLimiter.RateLimiter;
using RateLimiter.RateLimiter.Models;
using RateLimiter.RateLimiter.Options;
using System;
using System.Linq;

namespace RateLimiter
{
    public static class DependencyInjection
    {
        public static IApplicationBuilder UseRateLimiter(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitingMiddleware>();
        }

        public static IServiceCollection AddRateLimiter(
            this IServiceCollection services)
        {
            services.AddSingleton(x =>
            {
                var limiters = x.GetServices<Tuple<string, Region, ILimiter>>();

                return limiters.ToDictionary(x => (x.Item1, x.Item2), x => x.Item3);
            });

            services.AddSingleton<RateLimitProcessor>();

            return services;
        }

        public static IEndpointConventionBuilder RequireRateLimiting(
            this IEndpointConventionBuilder builder,
            string policyName,
            Region region)
        {
            builder.Add(endpointBuilder =>
            {
                endpointBuilder.Metadata.Add(new EnableRateLimitingAttribute(region, policyName));
            });

            return builder;
        }

        public static IServiceCollection AddFixedWindowRateLimiter(
            this IServiceCollection services,
            string policy,
            Region region,
            Action<FixedWindowLimiterOptions> configureOptions)
        {
            var options = new FixedWindowLimiterOptions();
            configureOptions.Invoke(options);

            ILimiter limiter = new FixedWindowLimiter(options);

            services.AddTransient(x => Tuple.Create(policy, region, limiter));

            return services;
        }

        public static IServiceCollection AddTimespanRateLimiter(
            this IServiceCollection services,
            string policy,
            Region region,
            Action<TimespanLimiterOptions> configureOptions)
        {
            var options = new TimespanLimiterOptions();
            configureOptions.Invoke(options);

            ILimiter limiter = new TimespanLimiter(options);

            services.AddTransient(x => Tuple.Create(policy, region, limiter));

            return services;
        }
    }
}
