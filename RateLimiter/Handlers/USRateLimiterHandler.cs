using RateLimiter.Statistics;

namespace RateLimiter.Handlers
{
    public class USRateLimiterHandler : IRateLimiterHandler
    {
        public bool IsExceeded(IClientStatistics statistics, RateLimiterAttribute options)
        {
            if (statistics is not null)
            {
                return DateTime.UtcNow < statistics.LastSuccessfulResponseTime.AddMilliseconds(options.Timespan) &&
                    statistics.NumberOfRequests >= options.MaxRequests;
            }

            return false;
        }
    }
}
