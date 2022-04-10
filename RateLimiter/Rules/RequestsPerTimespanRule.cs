using Microsoft.Extensions.Options;
using RateLimiter.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Rules
{
    public class RequestsPerTimespanRule : IRateLimiterRule
    {
        private readonly IApiClient apiClient;
        private readonly IRateLimiterRepository rateLimiterRepository;
        private readonly RequestPerTimeSpanOptions options;

        public RequestsPerTimespanRule(IApiClient apiClient,
                                   IRateLimiterRepository rateLimiterRepository,
                                   IOptions<RequestPerTimeSpanOptions> options)
        {
            this.apiClient = apiClient;
            this.rateLimiterRepository = rateLimiterRepository;
            this.options = options.Value;
        }

        public bool IsValid()
        {
            var numberOfRequests = rateLimiterRepository.GetAmountOfLoginsSinceTimespan(apiClient.ClientId, options.WithinTimeSpan);

            return numberOfRequests < options.MaxAlloweRequests;            
        }
    }
}
