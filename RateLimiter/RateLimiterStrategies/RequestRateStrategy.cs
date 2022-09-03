using RateLimiter.Models;
using RateLimiter.Models.Options;
using RateLimiter.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter.RateLimiterStrategies
{
    public class RequestRateStrategy : IRateLimiterStrategy
    {
        private readonly RequestRateOptions options;

        public RequestRateStrategy(RequestRateOptions options)
        {
            this.options = options;
        }

        public RateLimiterStrategyResponse Process(List<DateTime> requestTimes)
        {
            var mostRecent = requestTimes.Max();
            var startTime = mostRecent.Subtract(options.RequestTimespan);
            var requestsWithinTimespan = requestTimes.Count(ts => ts >= startTime);

            var processedClientRequest = new RateLimiterStrategyResponse(nameof(RequestRateStrategy));
            if (requestsWithinTimespan > options.MaxNumberOfRequests)
            {
                processedClientRequest.IsSuccess = false;
                processedClientRequest.Message = "Request rate exceeded";
            }

            return processedClientRequest;
        }
    }
}
