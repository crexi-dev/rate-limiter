using System;
using Microsoft.Extensions.Caching.Distributed;


namespace RateLimiter.Services
{
    public class LogApiHitCountService : ILogApiHitCountService
    {
        private readonly IDistributedCache _cache;
        public LogApiHitCountService(IDistributedCache cache)
        {
            _cache = cache;
        }

        // Gets and sets the api hit counts from cache

        public async Task<ApiHitCountLog> GetApiCounts(string key) 
        {
            var result =  await _cache.GetAsync(key);
            return result.FromByteArray<ApiHitCountLog>();
        }

        public async Task SaveApiCounts<ApiHitCountLog>(string key, ApiHitCountLog apiHitCountLog)
        {
            await _cache.SetAsync(key, apiHitCountLog.ToByteArray());
        }
    }
}

