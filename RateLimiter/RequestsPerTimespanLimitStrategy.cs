using RateLimiter.Contract;
using System.Threading.Tasks;

namespace RateLimiter
{
    internal class RequestsPerTimespanLimitStrategy : IRequestLimitStrategy
    {
        private readonly IRequestStatisticsDataSource requestStatisticsDataSource;
        private readonly RequestsPerTimespanLimitStrategyConfiguration requestsPerTimespanLimitStrategyConfiguration;
        private readonly IRateLimitCircuitBreaker rateLimitCircuitBreaker;
        private readonly IDateTimeProvider dateTimeProvider;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestStatisticsDataSource">Used to abstract away storage of the requests statistics. Storing it in memory can we costly when scaling to many clients. Alternative implementation - Redis cache.</param>
        /// <param name="requestsPerTimespanLimitStrategyConfiguration">Used to control how many requests can pass through for the defined timespan. Now it is global, but alternatively can be done on client basis (e.g. some clients may buy more expensive subscription allowing to perform more requests than the others)</param>
        public RequestsPerTimespanLimitStrategy(
            IRequestStatisticsDataSource requestStatisticsDataSource,
            RequestsPerTimespanLimitStrategyConfiguration requestsPerTimespanLimitStrategyConfiguration,
            IRateLimitCircuitBreaker rateLimitCircuitBreaker,
            IDateTimeProvider dateTimeProvider)
        {
            this.requestStatisticsDataSource = requestStatisticsDataSource;
            this.requestsPerTimespanLimitStrategyConfiguration = requestsPerTimespanLimitStrategyConfiguration;
            this.rateLimitCircuitBreaker = rateLimitCircuitBreaker;
            this.dateTimeProvider = dateTimeProvider;
        }

        public async Task<bool> CanPassThroughAsync(Request request)
        {
            if (this.rateLimitCircuitBreaker.IsClientLocked(request.ClientId))
            {
                return false;
            }

            var startTime = this.dateTimeProvider.UtcNow - this.requestsPerTimespanLimitStrategyConfiguration.TimeSpan;
            var endTime = this.dateTimeProvider.UtcNow;

            var numberOfRequestsPassedAlready = await this.requestStatisticsDataSource.GetNumberOfRequestsPassedAsync(request.ClientId, startTime, endTime);

            if (numberOfRequestsPassedAlready > this.requestsPerTimespanLimitStrategyConfiguration.MaxAllowedRequests)
            {
                this.rateLimitCircuitBreaker.LockClient(request.ClientId);

                return false;
            }

            return true;
        }
    }
}
