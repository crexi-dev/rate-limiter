using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace RateLimiter.RequestLimiterPolicy
{
    public class RequestLimiterMiddleWare
    {
        const string Key = "RequestLimiter";

        private readonly IMemoryCache _cache;
        private readonly RequestDelegate _next;
        private readonly RequestLimiterMiddleWareOptions _options;

        public RequestLimiterMiddleWare(RequestDelegate next,
                IMemoryCache cache,
                IOptions<RequestLimiterMiddleWareOptions> options)
        {
            _next = next;
            _cache = cache;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            RequestLimiterValue actualValue;
            if (_cache.TryGetValue(Key, out object? value) && value is RequestLimiterValue requestLimiterValue)
            {
                requestLimiterValue.Calls = requestLimiterValue.Calls.Where(c => c > DateTime.Now.AddMilliseconds(-_options.TimeSpan)).ToList();
                actualValue = requestLimiterValue;
            }
            else
            {
                actualValue = new RequestLimiterValue();
            }

            actualValue.Calls.Add(DateTime.Now);

            _cache.Set(Key, actualValue);

            if (actualValue.Calls.Count < _options.Amount)
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 429;  // too many requests
            }
        }
    }
}
