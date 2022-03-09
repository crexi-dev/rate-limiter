using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using RateLimiter.Cache;
using RateLimiter.Models.Domain;

namespace RateLimiter.Filters
{
    /// <summary>
    /// Resource filter used to cache the user's client metrics
    /// </summary>
    public class CacheClientMetricsFilter : IAsyncResourceFilter
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConfiguration _configuration;

        public CacheClientMetricsFilter(IDistributedCache distributedCache, IConfiguration configuration)
        {
            _distributedCache = distributedCache;
            _configuration = configuration;
        }

        /// <summary>
        /// Called asynchronously before the rest of the pipeline.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var accessToken = context.HttpContext.Request.Headers["Authorization"][0]["Bearer ".Length..];
            var dateTime = DateTime.UtcNow;

            var clientMetrics = await _distributedCache.Get<ClientMetrics>(accessToken);
            if (clientMetrics != null)
            {
                //Drop all timestamps in the set which occurred before one interval ago
                var interval = _configuration.GetValue<int>("Interval");
                var expiredRequests = clientMetrics.Requests
                    .TakeWhile(x => x.Timestamp <= dateTime.AddSeconds(-interval)).ToList();

                clientMetrics.Requests.ExceptWith(expiredRequests);
                clientMetrics.TotalRequests -= expiredRequests.Count;
                clientMetrics.TotalRequestUnits -= expiredRequests.Sum(x => x.Cost);
                clientMetrics.TotalSize -= expiredRequests.Sum(x => x.Size);
            }
            else
                clientMetrics = new ClientMetrics(accessToken, 0, 0L, 0);

            context.HttpContext.Items.Add("ClientMetrics", clientMetrics);
            context.HttpContext.Items.Add("DateTime", dateTime);

            await next();
        }
    }
}
