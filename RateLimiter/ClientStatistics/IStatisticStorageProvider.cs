using RateLimiter.Interfaces;

namespace RateLimiter.ClientStatistics;

public interface IStatisticStorageProvider
{
    public T GetStorage<T>() where T : class, IClientStatistics, new();
}