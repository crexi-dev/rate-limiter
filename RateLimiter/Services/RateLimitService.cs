using Microsoft.Extensions.Options;
using RateLimiter.Configs;
using RateLimiter.InMemoryStore;
using RateLimiter.Models;
using RateLimiter.Rules;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimitService : IRateLimitService
    {
        private readonly ICacheStore _cache;
        private readonly RateLimitConfigurationOptions _rules;

        public RateLimitService(
            ICacheStore cache,
            IOptions<RateLimitConfigurationOptions> rules)
        {
            _cache = cache;
            _rules = rules?.Value;
        }

        public async Task<bool> ValidateRequest(ClientRequest clientRequest)
        {
            // If checking rate limiter is disabled return true
            if (!_rules.Enabled)
            {
                return true;
            }

            // Get all rules related to current resource and client 
            var rules = GetAllRules(clientRequest);

            // If no rule was defined for current resource and client return true
            if (rules.Count == 0)
            {
                return true;
            }

            // Get all requests related to current client and resource
            await _cache.SetAsync(clientRequest);
            
            var requests = await _cache.GetAsync(clientRequest);

            // If Client has not yet made any request to current resource return true
            if (requests.Count == 0 || requests == null)
            {
                return true;
            }

            // Validate all rules and return false if any of them fails
            foreach (var rule in rules)
            {
                var isValid = rule.IsValid(requests);
                if (!isValid)
                {
                    return false;
                }
            }

            return true;
        }

        private List<RateLimitRule> GetAllRules(ClientRequest clientRequest)
        {
            var rules = new List<RateLimitRule>();
           
            if (_rules?.NumberOfRequestsPerTimespanRules != null)
            {
                rules.AddRange(_rules.NumberOfRequestsPerTimespanRules.Where(rule => IsMatchingRule(rule, clientRequest)));
            }
            
            if (_rules?.TimeSpanBetweenTwoRequestsRules != null)
            {
                rules.AddRange(_rules?.TimeSpanBetweenTwoRequestsRules.Where(rule => IsMatchingRule(rule, clientRequest)));
            }

            return rules;
        }

        private bool IsMatchingRule(RateLimitRule rule, ClientRequest clientRequest)
        {
            return rule.Resource == clientRequest.Resource && rule.Method == clientRequest.Method;
        }
    }
}
