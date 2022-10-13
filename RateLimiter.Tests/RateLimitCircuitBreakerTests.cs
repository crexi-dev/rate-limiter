using Moq;
using NUnit.Framework;
using RateLimiter.Contract;
using System;

namespace RateLimiter.Tests
{
    [TestFixture]
    internal class RateLimitCircuitBreakerTests
    {
        private Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();

        [Test]
        public void IsClientLocked_DoesntExistInListOfLockedClients_ReturnFalse()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var rateLimitCircuitBreakerConfiguration = new RateLimitCircuitBreakerConfiguration();

            var sut = new RateLimitCircuitBreaker(rateLimitCircuitBreakerConfiguration, this.dateTimeProviderMock.Object);

            // Act
            var actualValue = sut.IsClientLocked(clientId);

            // Assert
            Assert.IsFalse(actualValue);
        }

        [Test]
        public void IsClientLocked_ExistsInListOfLockedClientsAndLockPeriodPassed_ReturnFalse()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var rateLimitCircuitBreakerConfiguration = new RateLimitCircuitBreakerConfiguration
            {
                LockFor = TimeSpan.FromMinutes(5)
            };

            // Setup datetime for lock operation
            var lockedAt = new DateTime(2022, 1, 1, 1, 0, 0);
            this.dateTimeProviderMock.Setup(t => t.UtcNow).Returns(lockedAt);

            var sut = new RateLimitCircuitBreaker(rateLimitCircuitBreakerConfiguration, this.dateTimeProviderMock.Object);
            sut.LockClient(clientId);

            // Setup datetime for lock check operation
            var dateTimeNow = lockedAt.Add(rateLimitCircuitBreakerConfiguration.LockFor).AddMilliseconds(1); // + 1 ms after lock period
            this.dateTimeProviderMock.Setup(t => t.UtcNow).Returns(dateTimeNow);

            // Act
            var actualValue = sut.IsClientLocked(clientId);

            // Assert
            Assert.IsFalse(actualValue);
        }

        [Test]
        public void IsClientLocked_ExistsInListOfLockedClientsAndLockPeriodHaventPassed_ReturnTrue()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var rateLimitCircuitBreakerConfiguration = new RateLimitCircuitBreakerConfiguration
            {
                LockFor = TimeSpan.FromMinutes(5)
            };

            // Setup datetime for lock operation
            var lockedAt = new DateTime(2022, 1, 1, 1, 0, 0);
            this.dateTimeProviderMock.Setup(t => t.UtcNow).Returns(lockedAt);

            var sut = new RateLimitCircuitBreaker(rateLimitCircuitBreakerConfiguration, this.dateTimeProviderMock.Object);
            sut.LockClient(clientId);

            // Setup datetime for lock check operation
            var dateTimeNow = lockedAt.Add(rateLimitCircuitBreakerConfiguration.LockFor);
            this.dateTimeProviderMock.Setup(t => t.UtcNow).Returns(dateTimeNow);

            // Act
            var actualValue = sut.IsClientLocked(clientId);

            // Assert
            Assert.IsTrue(actualValue);
        }
    }
}
