using Moq;
using NUnit.Framework;
using RateLimiter.Contract;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    internal class RequestsPerTimespanLimitStrategyTests
    {
        private Mock<IRequestStatisticsDataSource> requestStatisticsDataSourceMock = new Mock<IRequestStatisticsDataSource>();
        private Mock<IRateLimitCircuitBreaker> rateLimitCircuitBreakerMock = new Mock<IRateLimitCircuitBreaker>();
        private Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();

        [Test]
        public async Task CanPassThroughAsync_CircuitBreakerClosed_ReturnFalse()
        {
            // Arrange
            var request = new Request
            {
                ClientId = Guid.NewGuid()
            };

            var requestsPerTimespanLimitStrategyConfiguration = new RequestsPerTimespanLimitStrategyConfiguration();

            this.rateLimitCircuitBreakerMock.Setup(t => t.IsClientLocked(request.ClientId)).Returns(true);

            var sut = new RequestsPerTimespanLimitStrategy(
                this.requestStatisticsDataSourceMock.Object,
                requestsPerTimespanLimitStrategyConfiguration,
                this.rateLimitCircuitBreakerMock.Object,
                this.dateTimeProviderMock.Object);

            // Act
            var actualValue = await sut.CanPassThroughAsync(request);

            // Assert
            Assert.IsFalse(actualValue);
        }

        [Test]
        public async Task CanPassThroughAsync_CircuitBreakerOpenedAndRateWasntExceeded_ReturnTrue()
        {
            // Arrange
            var request = new Request
            {
                ClientId = Guid.NewGuid()
            };

            var requestsPerTimespanLimitStrategyConfiguration = new RequestsPerTimespanLimitStrategyConfiguration
            {
                MaxAllowedRequests = 10,
                TimeSpan = TimeSpan.FromSeconds(1),
            };

            var dateTimeNow = new DateTime(2022, 1, 1, 12, 0, 0);
            this.dateTimeProviderMock.Setup(t => t.UtcNow).Returns(dateTimeNow);

            this.requestStatisticsDataSourceMock.Setup(t => t.GetNumberOfRequestsPassedAsync(request.ClientId, dateTimeNow.AddSeconds(-1), dateTimeNow)).ReturnsAsync(9);

            this.rateLimitCircuitBreakerMock.Setup(t => t.IsClientLocked(request.ClientId)).Returns(false);

            var sut = new RequestsPerTimespanLimitStrategy(
                this.requestStatisticsDataSourceMock.Object,
                requestsPerTimespanLimitStrategyConfiguration,
                this.rateLimitCircuitBreakerMock.Object,
                this.dateTimeProviderMock.Object);

            // Act
            var actualValue = await sut.CanPassThroughAsync(request);

            // Assert
            Assert.IsTrue(actualValue);
        }

        [Test]
        public async Task CanPassThroughAsync_CircuitBreakerOpenedAndRateExceeded_ReturnFalse()
        {
            // Arrange
            var request = new Request
            {
                ClientId = Guid.NewGuid()
            };

            var requestsPerTimespanLimitStrategyConfiguration = new RequestsPerTimespanLimitStrategyConfiguration
            {
                MaxAllowedRequests = 10,
                TimeSpan = TimeSpan.FromSeconds(1),
            };

            var dateTimeNow = new DateTime(2022, 1, 1, 12, 0, 0);
            this.dateTimeProviderMock.Setup(t => t.UtcNow).Returns(dateTimeNow);

            this.requestStatisticsDataSourceMock.Setup(t => t.GetNumberOfRequestsPassedAsync(request.ClientId, dateTimeNow.AddSeconds(-1), dateTimeNow)).ReturnsAsync(11);

            this.rateLimitCircuitBreakerMock.Setup(t => t.IsClientLocked(request.ClientId)).Returns(false);

            var sut = new RequestsPerTimespanLimitStrategy(
                this.requestStatisticsDataSourceMock.Object,
                requestsPerTimespanLimitStrategyConfiguration,
                this.rateLimitCircuitBreakerMock.Object,
                this.dateTimeProviderMock.Object);

            // Act
            var actualValue = await sut.CanPassThroughAsync(request);

            // Assert
            Assert.IsFalse(actualValue);
            this.rateLimitCircuitBreakerMock.Verify(t => t.LockClient(request.ClientId), Times.Once());
        }
    }
}
