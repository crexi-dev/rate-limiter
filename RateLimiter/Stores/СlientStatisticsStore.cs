using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Models;

namespace RateLimiter.Stores
{
    public sealed class СlientStatisticsStore
    {
        private readonly IMemoryCache cache;

        public СlientStatisticsStore(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public СlientStatistics Get(string clientId, string resourceName)
        {
            var key = GetKey(clientId, resourceName);
            var clientStatistics = cache.Get<СlientStatistics>(key) ?? new СlientStatistics(clientId, resourceName);
            return clientStatistics;
        }

        public void Set(СlientStatistics clientStatistics)
        {
            var key = GetKey(clientStatistics.ClientId, clientStatistics.ResourceName);
            cache.Set(key, clientStatistics);
        }

        private string GetKey(string clientId, string resourceName) => $"{resourceName} for {clientId}";
    }
}
