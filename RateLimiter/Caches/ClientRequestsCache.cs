using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Caches
{
    public class ClientRequestsCache : IClientRequestsCache
    {
        private const int CacheExpirationData = 100;
        private readonly IMemoryCache _memoryCache;

        public ClientRequestsCache(
            IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException((nameof(memoryCache)));
        }

        // TODO: skip mapper (use the same model)
        public async Task<bool> AddClientRequestDataAsync(string key, RequestsHistoryModel data)
        {
            List<RequestsHistoryModel> requests;

            if (!_memoryCache.TryGetValue(key, out requests))
            {
                requests = new List<RequestsHistoryModel>();                
            }

            requests.Add(data);
            // TODO: we can try to clear list with outdated requests.

            _memoryCache.Set(key, requests);

            return await Task.FromResult(true);
        }

        public async Task<List<RequestsHistoryModel>> GetClientRequestsDataAsync(string key)
        {
            var data = _memoryCache.Get<List<RequestsHistoryModel>>(key);

            return await Task.FromResult(data);
        }
    }
}
