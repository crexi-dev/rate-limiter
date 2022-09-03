using RateLimiter.Models;
using RateLimiter.RateLimiterProcessors;
using RateLimiter.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Services
{
    public class RateLimiterService
    {
        private readonly ICacheProvider clientRequestRepository;
        private readonly IList<IRateLimiterProcessor> rateLimiterProcessors = new List<IRateLimiterProcessor>();


        public RateLimiterService(ICacheProvider clientRequestRepository, IEnumerable<IRateLimiterProcessor> rateLimiterProcessors, Config config)
        {
            this.clientRequestRepository = clientRequestRepository;
            foreach (var name in config.ActiveProcessorNames)
            {
                this.rateLimiterProcessors = rateLimiterProcessors.Where(prc => prc.Name.ToString().Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        public List<RateLimiterStrategyResponse> ProcessRequest(string clientId, List<DateTime> requestTimes)
        {
            clientRequestRepository.Set(clientId, requestTimes);
            var responses = new List<RateLimiterStrategyResponse>();
            foreach (var processor in rateLimiterProcessors)
            {
                responses.Add(processor.Process(requestTimes));
            }

            return responses;
        }
    }
}
