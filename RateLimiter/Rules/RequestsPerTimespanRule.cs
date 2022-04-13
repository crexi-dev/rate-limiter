using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RateLimiter.Interfaces;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    /// <summary>
    /// This rule:
    /// 1. Is applicable for all requests coming from the US or an unknown region
    /// 2. Allows a limited number of requests per second. The number of allowed requests is given by the RequestsPerTimespanRule_MaxRequests
    /// configuration parameter
    /// </summary>
    public class RequestsPerTimespanRule : RateLimiterBaseRule
    {
        private ICachingService _cachingService;
        private readonly int _maxRequestsPerWindow;
        
        public RequestsPerTimespanRule(ICachingService cachingService, IConfiguration configuration)
        {
            _cachingService = cachingService;
            _maxRequestsPerWindow = int.Parse(configuration["RequestsPerTimespanRule_MaxRequests"]);
        }

        protected override Task<bool> IsApplicableAsync(RequestInfo request, CancellationToken cancellationToken)
        {
            // Applies for US based users OR users with unknown region
            return Task.FromResult(request.Location.Region == Region.US || request.Location.Region == Region.Unknown);
        }

        protected override async Task<bool> IsPermittedAsync(RequestInfo request, CancellationToken cancellationToken)
        {
            long counter = await _cachingService.UpdateValue<long>(this.GetCachingKey(request), 
                currentValue => currentValue + 1, 0, cancellationToken);
            return counter < _maxRequestsPerWindow;
        }

        /// <summary>
        /// Calculate the cache key for this particular request and time window
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual string GetCachingKey(RequestInfo request)
        {
            var windowId = request.DateTime.Ticks / 10000000; // 10,000,000 ticks per second.
            return typeof(TimeSinceLastRequestRule).Name + "_" + windowId.ToString() + "_" + this.GetIdForToken(request.Token);
        }
    }
}
