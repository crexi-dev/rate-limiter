namespace RateLimiter.Interfaces;

public interface IRateLimitRule
{
    public bool IsMatched(RequestInformation request);

    public IBucketIdentifier GetStatisticsId(RequestInformation request);
}


public interface IRateLimitRuleThresholdProvider<T> : IRateLimitRule
{
    public T Threshold { get; }
}