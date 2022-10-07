using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using RateLimiter.Configuration.Options;
using RateLimiter.Exceptions;
using RateLimiter.Services;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private const string TestLocationName = "EU";
        
        [Test]
        public void CheckLocationRequestCount_WhenRequestLimitIsNotReached_NoExceptionIsOccured()
        {
            // Arrange

            var autoMocker = new AutoMocker();
            var mockLimiterOptions = new Mock<IOptions<LimiterOptions>>();

            mockLimiterOptions
                .Setup(_ => _.Value)
                .Returns(new LimiterOptions
                {
                    LocationLimiters = new List<LocationLimiterOptions>
                    {
                        new LocationLimiterOptions
                        {
                            LocationName = "EU",
                            TimeRange = new TimeSpan(0, 0, 15),
                            AllowedRequestsCountPerTimeRange = 2
                        }
                    }
                });
            
            autoMocker.Use(mockLimiterOptions);

            var limiterStore = autoMocker.CreateInstance<LimiterStore>();

            // Act, Assert

            try
            {
                limiterStore.CheckLocationRequestCount(TestLocationName);
            }
            catch (AllowedRequestsCountReachedException)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void CheckLocationRequestCount_WhenRequestLimitIsReached_ThrowsAllowedRequestsCountReachedException()
        {
            // Arrange

            var autoMocker = new AutoMocker();
            var mockLimiterOptions = new Mock<IOptions<LimiterOptions>>();

            mockLimiterOptions
                .Setup(_ => _.Value)
                .Returns(new LimiterOptions
                {
                    LocationLimiters = new List<LocationLimiterOptions>
                    {
                        new LocationLimiterOptions
                        {
                            LocationName = "EU",
                            TimeRange = new TimeSpan(0, 0, 15),
                            AllowedRequestsCountPerTimeRange = 2
                        }
                    }
                });
            
            autoMocker.Use(mockLimiterOptions);

            var limiterStore = autoMocker.CreateInstance<LimiterStore>();

            // Act, Assert

            Assert.Throws<AllowedRequestsCountReachedException>(() =>
            {
                limiterStore.CheckLocationRequestCount(TestLocationName);
                limiterStore.CheckLocationRequestCount(TestLocationName);
                limiterStore.CheckLocationRequestCount(TestLocationName);
            });
        }

        [Test]
        public void CheckLocationRequestCount_WhenRequestLimitIsReachedAndResetAfterTime_NoExceptionIsOccured()
        {
            // Arrange

            var autoMocker = new AutoMocker();
            var mockLimiterOptions = new Mock<IOptions<LimiterOptions>>();

            mockLimiterOptions
                .Setup(_ => _.Value)
                .Returns(new LimiterOptions
                {
                    LocationLimiters = new List<LocationLimiterOptions>
                    {
                        new LocationLimiterOptions
                        {
                            LocationName = "EU",
                            TimeRange = new TimeSpan(0, 0, 2),
                            AllowedRequestsCountPerTimeRange = 2
                        }
                    }
                });
            
            autoMocker.Use(mockLimiterOptions);

            var limiterStore = autoMocker.CreateInstance<LimiterStore>();

            // Act, Assert

            Assert.Throws<AllowedRequestsCountReachedException>(() =>
            {
                limiterStore.CheckLocationRequestCount(TestLocationName);
                limiterStore.CheckLocationRequestCount(TestLocationName);
                limiterStore.CheckLocationRequestCount(TestLocationName);
            });
            
            Thread.Sleep(3000);
            
            try
            {
                limiterStore.CheckLocationRequestCount(TestLocationName);
            }
            catch (AllowedRequestsCountReachedException)
            {
                Assert.Fail();
            }
        }
    }
}
