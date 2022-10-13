using RateLimiter.Contract;
using System.Threading.Tasks;

namespace RateLimiter
{
    internal class DefaultRateLimiter : IRateLimiter
    {
        private readonly RequestLimitStrategyFactory requestLimiterStrategyFactory;
        private readonly IRequestStatisticsDataSource requestStatisticsDataSource;

        public DefaultRateLimiter(
            RequestLimitStrategyFactory requestLimiterStrategyFactory,
            IRequestStatisticsDataSource requestStatisticsDataSource)
        {
            this.requestLimiterStrategyFactory = requestLimiterStrategyFactory;
            this.requestStatisticsDataSource = requestStatisticsDataSource;
        }

        public async Task<bool> RateLimitExceeded(Request request)
        {
            var requestLimitStrategy = this.requestLimiterStrategyFactory.Create(request.ClientLocation);

            if (await requestLimitStrategy.CanPassThroughAsync(request))
            {
                await this.requestStatisticsDataSource.AddRequestAsync(request);

                return true;
            }

            return false;
        }

        // for testing purposes, should be exposed via DI container
        public static DefaultRateLimiter Instance = new DefaultRateLimiter(new RequestLimitStrategyFactory(), InMemoryRequestStatisticsDataSource.Instance);
    }
}
