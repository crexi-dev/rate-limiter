using RateLimiter.Models;
using RateLimiter.Models.Options;
using RateLimiter.RateLimiterProcessors.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.RateLimiterProcessors
{
    public class LastCallTimeSpanProcessor : IRateLimiterProcessor
    {
        private readonly LastCallTimeSpanOptions options;

        public LastCallTimeSpanProcessor(LastCallTimeSpanOptions options)
        {
            this.options = options;
        }

        public ProcessorName Name => ProcessorName.LastCallTimeSpan;

        public RateLimiterStrategyResponse Process(IList<DateTime> requestTimes)
        {
            var orderedRequestTimes = requestTimes.OrderByDescending(r => r).ToList();
            var second = orderedRequestTimes.FirstOrDefault();
            var first = orderedRequestTimes.Skip(1).FirstOrDefault();

            TimeSpan durationBetweenRequests = second.Subtract(first);

            var processedClientRequest = new RateLimiterStrategyResponse(nameof(RequestRateProcessor));
            if (durationBetweenRequests < options.MinRequestTimespan)
            {
                processedClientRequest.IsSuccess = false;
                processedClientRequest.Message = "Minimum timespan between requests violated";
            }

            return processedClientRequest;
        }
    }
}
