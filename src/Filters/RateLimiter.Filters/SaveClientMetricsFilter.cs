using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using RateLimiter.Cache;
using RateLimiter.Models.Contract;
using RateLimiter.Models.Domain;

namespace RateLimiter.Filters
{
    /// <summary>
    /// Action filter used to update the user's client metrics
    /// </summary>
    public class SaveClientMetricsFilter : IAsyncActionFilter
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConfiguration _configuration;

        public SaveClientMetricsFilter(IDistributedCache distributedCache, IConfiguration configuration)
        {
            _distributedCache = distributedCache;
            _configuration = configuration;
        }

        /// <summary>
        /// Called after the action executes.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionExecutedContext = await next();

            // execute after the action executes
            var interval = _configuration.GetValue<int>("Interval");

            var requestUnits = 0D;
            var contentLength = 0L;
            var objectResult = actionExecutedContext.Result as ObjectResult;
            if (objectResult?.Value != null)
            {
                var response = objectResult.Value as BaseResponse;
                requestUnits = response?.RequestCharge ?? 0D;
                contentLength = response?.Size ?? 0;
            }

            var dateTime = (DateTime)context.HttpContext.Items["DateTime"];
            var clientMetrics = (ClientMetrics)context.HttpContext.Items["ClientMetrics"];

            clientMetrics.Requests.Add(new Request(dateTime, contentLength, requestUnits));
            clientMetrics.TotalSize += contentLength;
            clientMetrics.TotalRequests++;
            clientMetrics.TotalRequestUnits += requestUnits;
            clientMetrics.ExpiresAt = dateTime.AddSeconds(interval);

            await _distributedCache.Insert(clientMetrics.AccessKey, clientMetrics, TimeSpan.FromSeconds(interval));
        }
    }
}
