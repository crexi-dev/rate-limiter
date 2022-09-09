using Common;
using Microsoft.Extensions.Caching.Distributed;

namespace RateLimiter.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IDistributedCache _cache;
        public StatisticsService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetClientStatistics<T>(string key, CancellationToken token = default(CancellationToken))
             where T : class, IClientStatistics
        {
            var result = await _cache.GetAsync(key, token);
            return result.FromByteArray<T>();
        }

        public async Task UpdateClientStatistics<T>(string key, T statistics, CancellationToken token = default(CancellationToken))
            where T : class, IClientStatistics
        {
            await _cache.SetAsync(key, statistics.ToByteArray(), token);
        }
    }
}
