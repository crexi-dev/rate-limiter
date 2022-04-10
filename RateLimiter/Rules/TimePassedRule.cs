using Microsoft.Extensions.Options;
using RateLimiter.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Rules
{
    public class TimePassedRule : IRateLimiterRule
    {
        private readonly IApiClient apiClient;
        private readonly IRateLimiterRepository rateLimiterRepository;
        private readonly IOptions<TimePassedRuleOptions> options;

        public TimePassedRule(IApiClient apiClient,
                              IRateLimiterRepository rateLimiterRepository,
                              IOptions<TimePassedRuleOptions> options)
        {
            this.apiClient = apiClient;
            this.rateLimiterRepository = rateLimiterRepository;
            this.options = options;
        }

        public bool Validate()
        {
            var lastLoginDateTime = rateLimiterRepository.GetLastLoginDateTime(apiClient.ClientId);

            // if this is the first time client is logging then return true.
            if (!lastLoginDateTime.HasValue) return true;

            var span = DateTime.Now - lastLoginDateTime.Value;

            if (span > options.Value.MinTimespan)
            {
                return false;
            }

            return true;
        }
    }
}
