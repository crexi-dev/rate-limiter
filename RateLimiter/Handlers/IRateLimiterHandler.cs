using RateLimiter.Statistics;

namespace RateLimiter.Handlers
{
    public interface IRateLimiterHandler
    {
        bool IsExceeded(IClientStatistics statistics, RateLimiterAttribute options);
    }
}
