namespace RateLimiter.InMemoryCache.Interfaces
{
    public interface IInMemoryCacheProxy
    {
        void AddOrUpdateEntity<T>(string key, T entity)
            where T : class;
        T GetEntity<T>(string key)
            where T : class;
    }
}
