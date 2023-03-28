namespace RateLimiter.Interfaces;

public interface IClientStatistics
{
    public void IncrementStatisticsForRequest(IBucketIdentifier clientId, ITimeProvider timeProvider);
}

public interface IClientStatisticsProvider<T>{
    public T GetStatistics(IBucketIdentifier clientId);
}