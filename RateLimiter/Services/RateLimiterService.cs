using RateLimiter.Models;
using RateLimiter.RateLimiterStrategies;
using RateLimiter.Repositories;
using System;
using System.Collections.Generic;

namespace RateLimiter.Services
{
    public class RateLimiterService
    {
        private readonly IClientRequestRepository clientRequestRepository;
        private readonly List<IRateLimiterStrategy> rateLimiterStrategies;


        public RateLimiterService(IClientRequestRepository clientRequestRepository, List<IRateLimiterStrategy> rateLimiterStrategies)
        {
            this.clientRequestRepository = clientRequestRepository;
            this.rateLimiterStrategies = rateLimiterStrategies;
        }

        public List<RateLimiterStrategyResponse> ProcessRequest(string clientId, DateTime requestTime)
        {
            var requestTimes = clientRequestRepository.Add(clientId, requestTime);
            var responses = new List<RateLimiterStrategyResponse>();
            foreach (var strategy in rateLimiterStrategies)
            {
                responses.Add(strategy.Process(requestTimes));
            }

            return responses;
        }
    }
}
