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
        private readonly IOptions<RulesOptions> options;

        public RequestsPerTimespanRule(IApiClient apiClient,
                                   IRateLimiterRepository rateLimiterRepository,
                                   IOptions<RulesOptions> options)
        {
            this.apiClient = apiClient;
            this.rateLimiterRepository = rateLimiterRepository;
            this.options = options;
        }

        public bool IsValid()
        {

            throw new NotImplementedException();
            
        }
    }
}
