using System;
using RateLimiter.Client;
using RateLimiter.Library;
using RateLimiter.Library.Repository;
using NSubstitute;
using NUnit.Framework;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public void Verify_True_WithinRatelimit()
        {
            // arrange 
            var clientToken = "abc123";
            var serverIP = "183.28.89.21";
            var requestDate = new DateTime(2020, 1, 1, 0, 0, 0, 500);   // 1/1/2020 12:00:05AM
            var lastUpdateDate = new DateTime(2020, 1, 1, 0, 0, 0, 0);   // 1/1/2020 12:00:00AM

            var fakeClientRepository = Substitute.For<IClientRepository>();

            var rateLimiterProxy = Substitute.For<IRateLimiterProxy>();
            var rateLimitSettingsConfig = new RateLimitSettingsConfig();
            var rateLimitSettings = new RequestsPerTimespanSettings()
            {
                MaxAmount = 5
            };

            rateLimiterProxy.Verify(clientToken, requestDate).Returns(true);
            var rateLimiterClient = new RateLimiterClient(rateLimiterProxy);

            // act
            var result = rateLimiterClient.Verify(clientToken, requestDate);

            // assert
            Assert.AreEqual(result, true);
        }

        [Test]
        public void Verify_False_ExceedRatelimit()
        {
            // arrange 
            var clientToken = "abc123";
            var serverIP = "183.28.89.21";
            var requestDate = new DateTime(2020, 1, 1, 0, 0, 0, 500);   // 1/1/2020 12:00:05AM
            var lastUpdateDate = new DateTime(2020, 1, 1, 0, 0, 0, 0);   // 1/1/2020 12:00:00AM

            var fakeClientRepository = Substitute.For<IClientRepository>();

            var rateLimiterProxy = Substitute.For<IRateLimiterProxy>();
            rateLimiterProxy.Verify(clientToken, requestDate, new RateLimitSettingsConfig()).Returns(false);
            var rateLimiterClient = new RateLimiterClient(rateLimiterProxy);

            // act
            var result = rateLimiterClient.Verify(clientToken, requestDate, new RateLimitSettingsConfig());

            // assert
            Assert.AreEqual(result, false);
        }
    }
}
