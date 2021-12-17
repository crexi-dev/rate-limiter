using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimiterMiddlware
    {
        private readonly RequestDelegate _next;
        static readonly ConcurrentDictionary<string, DateTime?> ApiCallsInMemory = new();
        public RateLimiterMiddlware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            int timeSpan = 1; //this value shoud get from configuration for each instance
            string key = "";

            var previousApiCall = GetPreviousApiCallByKey(key);
            if (previousApiCall != null)
            {

                if (DateTime.Now < previousApiCall.Value.AddSeconds(timeSpan))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    return;
                }
            }

            UpdateApiCallFor(key);

            await _next(context);
        }

        private void UpdateApiCallFor(string key)
        {
            ApiCallsInMemory.TryRemove(key, out _);
            ApiCallsInMemory.TryAdd(key, DateTime.Now);
        }

        private DateTime? GetPreviousApiCallByKey(string key)
        {
            ApiCallsInMemory.TryGetValue(key, out DateTime? value);
            return value;
        }
    }

}
