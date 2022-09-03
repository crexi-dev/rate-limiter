using RateLimiter.Models;
using RateLimiter.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.RateLimiterStrategies
{
    public class LastCallTimeSpanStrategy : IRateLimiterStrategy
    {
        private readonly LastCallTimeSpanOptions options;

        public LastCallTimeSpanStrategy(LastCallTimeSpanOptions options)
        {
            this.options = options;
        }

        public RateLimiterStrategyResponse Process(List<DateTime> requestTimes)
        {
            var orderedRequestTimes = requestTimes.OrderByDescending(r => r).ToList();
            var second = requestTimes.OrderByDescending(r => r).FirstOrDefault();
            var first = requestTimes.OrderByDescending(r => r).Skip(1).FirstOrDefault();

            TimeSpan durationBetweenRequests = second.Subtract(first);

            var processedClientRequest = new RateLimiterStrategyResponse(nameof(RequestRateStrategy));
            if (durationBetweenRequests < options.MinRequestTimespan)
            {
                processedClientRequest.IsSuccess = false;
                processedClientRequest.Message = "Minimum timespan between requests violated";
            }

            return processedClientRequest;
        }
    }
}
