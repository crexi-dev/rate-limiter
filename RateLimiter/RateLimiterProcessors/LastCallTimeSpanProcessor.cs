using Microsoft.Extensions.Options;
using RateLimiter.Models;
using RateLimiter.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.RateLimiterProcessors
{
    public class LastCallTimeSpanProcessor : IRateLimiterProcessor
    {
        private readonly LastCallTimeSpanOptions options;

        public LastCallTimeSpanProcessor(IOptions<LastCallTimeSpanOptions> options)
        {

            this.options = options.Value;
        }

        public ProcessorName Name => ProcessorName.LastCallTimeSpan;

        public RateLimiterProcessorResponse Process(IList<DateTime> requestTimes)
        {
            var orderedRequestTimes = requestTimes.OrderByDescending(r => r).ToList();
            var second = orderedRequestTimes.FirstOrDefault();
            var first = orderedRequestTimes.Skip(1).FirstOrDefault();

            TimeSpan durationBetweenRequests = second.Subtract(first);

            var processedClientRequest = new RateLimiterProcessorResponse(nameof(RequestRateProcessor));
            if (durationBetweenRequests < options.MinRequestTimespanInMilliseconds)
            {
                processedClientRequest.IsSuccess = false;
                processedClientRequest.Message = "Minimum timespan between requests violated";
            }

            return processedClientRequest;
        }
    }
}
