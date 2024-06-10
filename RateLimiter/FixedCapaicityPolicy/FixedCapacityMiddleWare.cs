using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace RateLimiter.FixedCapaicityPolicy
{
    public class FixedCapacityMiddleWare
    {
        const string Key = "FixedCapacity";
        const int DefaultTimeSpan = 1000;

        private readonly IMemoryCache _cache;
        private readonly RequestDelegate _next;
        private readonly int _timeSpan;

        public FixedCapacityMiddleWare(RequestDelegate next,
                IMemoryCache cache,
                IConfiguration configuration)
        {
            _next = next;
            _cache = cache;
            if (int.TryParse(configuration["RequestLimiterValue"], out var value))
            {
                _timeSpan = value;
            }
            else
            {
                _timeSpan = DefaultTimeSpan;
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            FixedCapacityValue actualValue = _cache.TryGetValue(Key, out object? value) && value is FixedCapacityValue requestLimiterValue
                ? requestLimiterValue
                : new FixedCapacityValue();

            if (actualValue.LastRequest < DateTime.Now.AddMilliseconds(-_timeSpan))
            {
                actualValue.LastRequest = DateTime.Now;
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 425;  // too early
            }

            _cache.Set(Key, actualValue);
        }
    }
}
