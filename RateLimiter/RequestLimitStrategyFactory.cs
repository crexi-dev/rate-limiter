using RateLimiter.Contract;
using System;

namespace RateLimiter
{
    // In real world this all should be in DI container, for simplicity use factory
    internal class RequestLimitStrategyFactory
    {
        public IRequestLimitStrategy Create(ClientLocation clientLocation)
        {
            switch (clientLocation)
            {
                case ClientLocation.US:
                    return new RequestsPerTimespanLimitStrategy(
                        InMemoryRequestStatisticsDataSource.Instance,
                        new RequestsPerTimespanLimitStrategyConfiguration { MaxAllowedRequests = 100, TimeSpan = TimeSpan.FromSeconds(10) },
                        new RateLimitCircuitBreaker(new RateLimitCircuitBreakerConfiguration { LockFor = TimeSpan.FromSeconds(1) }, new DateTimeProvider()),
                        new DateTimeProvider());
                case ClientLocation.UK:
                    return new SinceLastCallLimitStrategy(
                        InMemoryRequestStatisticsDataSource.Instance,
                        new SinceLastCallLimitStrategyConfiguration { TimeSpan = TimeSpan.FromMilliseconds(50) },
                        new DateTimeProvider());
                case ClientLocation.CH:
                    var requestsPerTimespanLimitStrategy = new RequestsPerTimespanLimitStrategy(
                        InMemoryRequestStatisticsDataSource.Instance,
                        new RequestsPerTimespanLimitStrategyConfiguration { MaxAllowedRequests = 100, TimeSpan = TimeSpan.FromSeconds(10) },
                        new RateLimitCircuitBreaker(new RateLimitCircuitBreakerConfiguration { LockFor = TimeSpan.FromSeconds(1) }, new DateTimeProvider()),
                        new DateTimeProvider());

                    var sinceLastCallLimitStrategy = new SinceLastCallLimitStrategy(
                        InMemoryRequestStatisticsDataSource.Instance,
                        new SinceLastCallLimitStrategyConfiguration { TimeSpan = TimeSpan.FromMilliseconds(50) },
                        new DateTimeProvider());

                    return new CompositeLimitStrategy(requestsPerTimespanLimitStrategy, sinceLastCallLimitStrategy);
                default:
                    return new RequestsPerTimespanLimitStrategy(
                        InMemoryRequestStatisticsDataSource.Instance,
                        new RequestsPerTimespanLimitStrategyConfiguration { MaxAllowedRequests = 100, TimeSpan = TimeSpan.FromSeconds(1) },
                        new RateLimitCircuitBreaker(new RateLimitCircuitBreakerConfiguration { LockFor = TimeSpan.FromSeconds(5) }, new DateTimeProvider()),
                        new DateTimeProvider());
            }

            
        }
    }
}
