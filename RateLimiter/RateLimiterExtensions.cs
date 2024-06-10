using Microsoft.AspNetCore.Builder;

using RateLimiter.FixedCapacityByCountryPolicy;
using RateLimiter.FixedCapaicityPolicy;
using RateLimiter.RequestLimiterPolicy;

namespace RateLimiter
{
    public static class RateLimiterExtensions
    {
        public static IApplicationBuilder UseRequestLimiterPolicy(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLimiterMiddleWare>();
        }

        public static IApplicationBuilder UseFixedCapacityPolicy(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FixedCapacityMiddleWare>();
        }

        public static IApplicationBuilder UseFixedCapacityByCountryPolicy(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FixedCapacityByCountryMiddleWare>();
        }
    }
}
