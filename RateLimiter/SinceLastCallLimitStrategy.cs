using RateLimiter.Contract;
using System.Threading.Tasks;

namespace RateLimiter
{
    internal class SinceLastCallLimitStrategy : IRequestLimitStrategy
    {
        private readonly IRequestStatisticsDataSource requestStatisticsDataSource;
        private readonly SinceLastCallLimitStrategyConfiguration sinceLastCallLimitStrategyConfiguration;
        private readonly IDateTimeProvider dateTimeProvider;

        public SinceLastCallLimitStrategy(
            IRequestStatisticsDataSource requestStatisticsDataSource,
            SinceLastCallLimitStrategyConfiguration sinceLastCallLimitStrategyConfiguration,
            IDateTimeProvider dateTimeProvider)
        {
            this.requestStatisticsDataSource = requestStatisticsDataSource;
            this.sinceLastCallLimitStrategyConfiguration = sinceLastCallLimitStrategyConfiguration;
            this.dateTimeProvider = dateTimeProvider;
        }

        public async Task<bool> CanPassThroughAsync(Request request)
        {
            var mostRecentRequest = await this.requestStatisticsDataSource.GetMostRecentRequestAsync(request.ClientId);

            if (mostRecentRequest != null)
            {
                return (this.dateTimeProvider.UtcNow - mostRecentRequest.DateTime) > this.sinceLastCallLimitStrategyConfiguration.TimeSpan;
            }

            return true;
        }
    }
}
