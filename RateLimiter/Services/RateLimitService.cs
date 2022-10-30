using RateLimiter.Caches;
using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly IEnumerable<IBaseRule> _rules;
        private readonly IClientRequestsCache _clientRequestsCache;

        public RateLimitService(
            IClientRequestsCache clientRequestsCache,
            IEnumerable<IBaseRule> rules)
        {
            _clientRequestsCache = clientRequestsCache;
            _rules = rules;
        }

        public async Task<bool> ValidateRequestAsync(string key, string region, IList<RequestAttributeDataModel> data)
        {
            if (data is null || !data.Any())
            {
                return await Task.FromResult(true);
            }

            var dataHistory = await _clientRequestsCache.GetClientRequestsDataAsync(key);

            // TODO: need more validation
            foreach (var limitationData in data.Where(e => e.RequestedRegions is null || !e.RequestedRegions.Any() || e.RequestedRegions.Contains(region)))
            {
                var rule = GetRule(limitationData.RequestedType);

                var result = rule.Validate(limitationData, dataHistory);

                if (!result)
                {
                    return result;
                }
            }

            return true;
        }

        public async Task<bool> AddClientRequestAsync(string key, string region)
        {
            return await _clientRequestsCache.AddClientRequestDataAsync(key, new RequestsHistoryModel
            {
                DateTime = DateTime.UtcNow,
                RegionName = region
            });
        }

        private IBaseRule GetRule(RateLimiterType type)
        {
            var rule = _rules.FirstOrDefault(e => e.RuleType == type);

            if (rule is null)
            {
                throw new NotImplementedException($"The {type} not impemented.");
            }

            return rule;
        }
    }
}
