using Microsoft.Extensions.Options;
using RateLimiter.Models;
using RateLimiter.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.RateLimiterProcessors
{
    public class RequestRateProcessor : IRateLimiterProcessor
    {
        private readonly RequestRateOptions options;

        public RequestRateProcessor(IOptions<RequestRateOptions> options)
        {
            this.options = options.Value;
        }

        public ProcessorName Name => ProcessorName.RequestRate;

        public RateLimiterProcessorResponse Process(IList<DateTime> requestTimes)
        {
            var mostRecent = requestTimes.Max();
            var startTime = mostRecent.Subtract(options.RequestTimespanInMilliseconds);
            var requestsWithinTimespan = requestTimes.Count(ts => ts >= startTime);

            var processedClientRequest = new RateLimiterProcessorResponse(nameof(RequestRateProcessor));
            if (requestsWithinTimespan > options.MaxNumberOfRequests)
            {
                processedClientRequest.IsSuccess = false;
                processedClientRequest.Message = "Request rate exceeded";
            }

            return processedClientRequest;
        }
    }
}
