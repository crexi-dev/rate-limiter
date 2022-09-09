namespace RateLimiter.Statistics
{
    public interface IStatisticsService
    {
        Task<T> GetClientStatistics<T>(string key, CancellationToken token = default(CancellationToken))
            where T : class, IClientStatistics;

        Task UpdateClientStatistics<T>(string key, T statistics, CancellationToken token = default(CancellationToken))
            where T : class, IClientStatistics;
    }
}
