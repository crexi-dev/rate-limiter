using RateLimiter.Statistics;

namespace RateLimiter.Handlers
{
    public class EURateLimiterHandler : IRateLimiterHandler
    {
        public bool IsExceeded(IClientStatistics statistics, RateLimiterAttribute options)
        {
            if (statistics is not null)
            {
                return DateTime.UtcNow < statistics.LastSuccessfulResponseTime.AddMilliseconds(options.Timespan);
            }

            return false;
        }
    }
}