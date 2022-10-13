using Moq;
using NUnit.Framework;
using RateLimiter.Contract;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    internal class SinceLastCallLimitStrategyTests
    {
        private Mock<IRequestStatisticsDataSource> requestStatisticsDataSourceMock = new Mock<IRequestStatisticsDataSource>();
        private Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();

        [Test]
        public async Task CanPassThroughAsync_VeryFirstCall_ReturnTrue()
        {
            // Arrange
            var request = new Request
            {
                ClientId = Guid.NewGuid()
            };

            var sinceLastCallLimitStrategyConfiguration = new SinceLastCallLimitStrategyConfiguration
            {
                TimeSpan = TimeSpan.FromMilliseconds(10)
            };

            var sut = new SinceLastCallLimitStrategy(
                this.requestStatisticsDataSourceMock.Object,
                sinceLastCallLimitStrategyConfiguration,
                this.dateTimeProviderMock.Object);

            // Act
            var actualValue = await sut.CanPassThroughAsync(request);

            // Assert
            Assert.IsTrue(actualValue);
        }

        [Test]
        public async Task CanPassThroughAsync_TimespanPassedSinceLastCall_ReturnTrue()
        {
            // Arrange
            var request = new Request
            {
                ClientId = Guid.NewGuid()
            };

            var sinceLastCallLimitStrategyConfiguration = new SinceLastCallLimitStrategyConfiguration
            {
                TimeSpan = TimeSpan.FromMilliseconds(10)
            };

            var currentDateTime = new DateTime(2022, 1, 1, 12, 0, 0);
            this.dateTimeProviderMock.Setup(t => t.UtcNow).Returns(currentDateTime);

            var latestRequestDateTime = currentDateTime.Add(-sinceLastCallLimitStrategyConfiguration.TimeSpan).AddMilliseconds(-1);

            this.requestStatisticsDataSourceMock.Setup(t => t.GetMostRecentRequestAsync(request.ClientId)).ReturnsAsync(new Request { DateTime = latestRequestDateTime });

            var sut = new SinceLastCallLimitStrategy(
                this.requestStatisticsDataSourceMock.Object,
                sinceLastCallLimitStrategyConfiguration,
                this.dateTimeProviderMock.Object);

            // Act
            var actualValue = await sut.CanPassThroughAsync(request);

            // Assert
            Assert.IsTrue(actualValue);
        }

        [Test]
        public async Task CanPassThroughAsync_TimespanDidntPassSinceLastCall_ReturnFalse()
        {
            // Arrange
            var request = new Request
            {
                ClientId = Guid.NewGuid()
            };

            var sinceLastCallLimitStrategyConfiguration = new SinceLastCallLimitStrategyConfiguration
            {
                TimeSpan = TimeSpan.FromMilliseconds(10)
            };

            var currentDateTime = new DateTime(2022, 1, 1, 12, 0, 0);
            this.dateTimeProviderMock.Setup(t => t.UtcNow).Returns(currentDateTime);

            var latestRequestDateTime = currentDateTime.Add(-sinceLastCallLimitStrategyConfiguration.TimeSpan).AddMilliseconds(1);

            this.requestStatisticsDataSourceMock.Setup(t => t.GetMostRecentRequestAsync(request.ClientId)).ReturnsAsync(new Request { DateTime = latestRequestDateTime });

            var sut = new SinceLastCallLimitStrategy(
                this.requestStatisticsDataSourceMock.Object,
                sinceLastCallLimitStrategyConfiguration,
                this.dateTimeProviderMock.Object);

            // Act

            var actualValue = await sut.CanPassThroughAsync(request);

            // Assert
            Assert.IsFalse(actualValue);
        }
    }
}
