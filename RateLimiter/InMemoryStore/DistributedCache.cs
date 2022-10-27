using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RateLimiter.Configs;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.InMemoryStore
{
    public class DistributedCache : ICacheStore
    {
        private readonly IDistributedCache _cache;
        private readonly TimeSpan? _expirationTime;

        public DistributedCache(IDistributedCache cache, IOptions<RateLimitConfigurationOptions> rules)
        {
            _cache = cache;
            _expirationTime = rules?.Value.ExpirationTime;
        }

        public async Task SetAsync(ClientRequest clientRequest)
        {
            var options = new DistributedCacheEntryOptions();

            if (_expirationTime.HasValue)
            {
                options.SetAbsoluteExpiration(_expirationTime.Value);
            }

            var requestsHistory = await GetAsync(clientRequest);

            if (requestsHistory == null || requestsHistory.Count == 0)
            {
                requestsHistory = new List<DateTime> { clientRequest.RequestTime };
            }
            else
            {
                requestsHistory.Add(clientRequest.RequestTime);
            }

            requestsHistory = requestsHistory.Where(r => DateTime.UtcNow - r <= _expirationTime).ToList();

            await _cache.SetStringAsync(
                $"{clientRequest.ClientId}-{clientRequest.Method}-{clientRequest.Resource}", 
                JsonConvert.SerializeObject(requestsHistory), 
                options);
        }

        public async Task<List<DateTime>> GetAsync(ClientRequest clientRequest)
        {
            var stored = await _cache.GetStringAsync($"{clientRequest.ClientId}-{clientRequest.Method}-{clientRequest.Resource}");

            if (!string.IsNullOrEmpty(stored))
            {
                return JsonConvert.DeserializeObject<List<DateTime>>(stored);
            }

            return new List<DateTime>();
        }
    }
}

