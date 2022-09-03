using RateLimiter.Models;
using RateLimiter.Models.Options;
using RateLimiter.RateLimiterProcessors.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.RateLimiterProcessors
{
    public class RequestRateProcessor : IRateLimiterProcessor
    {
        private readonly RequestRateOptions options;

        public RequestRateProcessor(RequestRateOptions options)
        {
            this.options = options;
        }

        public ProcessorName Name => ProcessorName.RequestRate;

        public RateLimiterStrategyResponse Process(IList<DateTime> requestTimes)
        {
            var mostRecent = requestTimes.Max();
            var startTime = mostRecent.Subtract(options.RequestTimespan);
            var requestsWithinTimespan = requestTimes.Count(ts => ts >= startTime);

            var processedClientRequest = new RateLimiterStrategyResponse(nameof(RequestRateProcessor));
            if (requestsWithinTimespan > options.MaxNumberOfRequests)
            {
                processedClientRequest.IsSuccess = false;
                processedClientRequest.Message = "Request rate exceeded";
            }

            return processedClientRequest;
        }
    }
}
