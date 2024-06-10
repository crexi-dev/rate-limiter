using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;


namespace RateLimiter.FixedCapacityByCountryPolicy
{
    public class FixedCapacityByCountryMiddleWare
    {
        const string Key = "FixedCapacityByCountry";

        private readonly IMemoryCache _cache;
        private readonly RequestDelegate _next;
        private readonly FixedCapacityByCountryMiddleWareOptions _options;

        public FixedCapacityByCountryMiddleWare(RequestDelegate next,
                IMemoryCache cache,
                IOptions<FixedCapacityByCountryMiddleWareOptions> options)
        {
            _next = next;
            _cache = cache;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var tokenType = GetTokenType(context);
            var options = GetOptionsByTokenType(tokenType);
            FixedCapacityByCountryValue actualValue = _cache.TryGetValue(Key, out object? value) && value is FixedCapacityByCountryValue requestLimiterValue
                ? requestLimiterValue
                : new FixedCapacityByCountryValue();


            if (actualValue.Calls.TryGetValue(tokenType, out var items))
            {
                var calls = items.Where(d => d > DateTime.Now.AddMilliseconds(-options.TimeSpan)).ToList();
                actualValue.Calls.Remove(tokenType);

                if (calls.Count < options.Amount)
                {
                    calls.Add(DateTime.Now);

                    actualValue.Calls.Add(tokenType, calls);

                    await _next(context);
                }
                else
                {
                    context.Response.StatusCode = 429;
                }
            }
            else
            {
                var calls = new List<DateTime>
                {
                    DateTime.Now,
                };

                actualValue.Calls.Add(tokenType, calls);

                await _next(context);
            }

            _cache.Set(Key, actualValue);
        }

        private FixedCapacityByCountryItemMiddleWareOptions GetOptionsByTokenType(string tokenType)
        {
            var options = _options.Items.Where(c => c.Name == tokenType).FirstOrDefault();
            if (options == null)
            {
                return new FixedCapacityByCountryItemMiddleWareOptions
                {
                    Amount = 10,
                    TimeSpan = 15000
                };
            }

            return options;
        }

        //TODO: logic to determine the origin of the token
        private string GetTokenType(HttpContext context)
        {
            var token = context.Request.Headers[""];
            if (string.IsNullOrEmpty(token))
            {
                return "Default";
            }

            return token;
        }
    }
}
