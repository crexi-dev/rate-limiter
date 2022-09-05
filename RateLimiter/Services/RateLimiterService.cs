using Microsoft.Extensions.Options;
using RateLimiter.Models;
using RateLimiter.Models.Options;
using RateLimiter.RateLimiterProcessors;
using RateLimiter.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Services
{
    public class RateLimiterService : IRateLimiterService
    {
        private readonly ICacheProvider cache;
        private readonly IList<IRateLimiterProcessor> rateLimiterProcessors = new List<IRateLimiterProcessor>();

        public RateLimiterService(ICacheProvider cache, IEnumerable<IRateLimiterProcessor> rateLimiterProcessors, IOptions<ActiveProcessorsOptions>? options = null)
        {
            this.cache = cache;
            var names = options?.Value.ActiveProcessorNames;
            foreach (var name in options?.Value.ActiveProcessorNames)
            {
                this.rateLimiterProcessors = rateLimiterProcessors
                    .Where(prc => prc.Name.ToString().Equals(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        public IList<RateLimiterProcessorResponse> ProcessRequest(string clientId, DateTime newRequestTime)
        {
            var requestTimes = cache.Get<List<DateTime>>(clientId);
            if (requestTimes == null)
            {
                requestTimes = new List<DateTime>();
            }

            requestTimes.Add(newRequestTime);
            cache.Set(clientId, requestTimes);

            var responses = new List<RateLimiterProcessorResponse>();
            foreach (var processor in rateLimiterProcessors)
            {
                responses.Add(processor.Process(requestTimes));
            }

            return responses;
        }
    }
}
