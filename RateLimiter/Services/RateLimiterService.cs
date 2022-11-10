using RateLimiter.DataCaching;
using RateLimiter.Enums;
using RateLimiter.Interfaces;
using RateLimiter.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public class RateLimiterService : IRateLimiterService
    {
        private readonly ICacheService _cacheService;
        private readonly IRulesRepository _rulesRepository;
        private readonly List<DateTime> _requestDates = new();

        public RateLimiterService(ICacheService cacheService,
            IRulesRepository rulesRepository)
        {
            _cacheService = cacheService;
            _rulesRepository = rulesRepository;
        }

        public async Task<bool> ValidateRequestAsync(
            string clientId, DateTime requestDate, string endpoint, Location location)
        {
            var rule = _rulesRepository.GetRule(endpoint, location);
            if (rule is null)
            {
                return true;
            }

            _cacheService.SetData(
                key: clientId.ToString(),
                value: requestDate,
                expirationTime: TimeSpan.FromMinutes(15));

            var date = _cacheService.GetData<DateTime>(clientId);

            if (date != DateTime.MinValue)
            {
                _requestDates.Add(date);
            }

            var result = await rule.ValidateAsync(_requestDates);
            if (!result)
            {
                return false;
            }

            return await Task.FromResult(true);
        }
    }
}
