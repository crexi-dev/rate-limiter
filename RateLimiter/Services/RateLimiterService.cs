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
        private readonly ICacheProvider clientRequestRepository;
        private readonly IList<IRateLimiterProcessor> rateLimiterProcessors = new List<IRateLimiterProcessor>();
        private readonly IOptions<ActiveProcessorsOptions>? options;


        public RateLimiterService(ICacheProvider clientRequestRepository, IEnumerable<IRateLimiterProcessor> rateLimiterProcessors, IOptions<ActiveProcessorsOptions>? options = null)
        {
            this.clientRequestRepository = clientRequestRepository;
            this.options = options;
            foreach (var name in options?.Value.ActiveProcessorNames)
            {
                this.rateLimiterProcessors = rateLimiterProcessors
                    .Where(prc => prc.Name.ToString().Equals(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        public IList<RateLimiterProcessorResponse> ProcessRequest(string clientId)
        {
            var requestTimes = clientRequestRepository.Get<List<DateTime>>(clientId);
            clientRequestRepository.Set(clientId, requestTimes);
            var responses = new List<RateLimiterProcessorResponse>();
            foreach (var processor in rateLimiterProcessors)
            {
                responses.Add(processor.Process(requestTimes));
            }

            return responses;
        }
    }
}
