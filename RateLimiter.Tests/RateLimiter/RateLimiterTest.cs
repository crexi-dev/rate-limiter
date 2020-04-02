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
        private RateLimitSettingsConfig defaultSettingsConfig;

        [SetUp]
        public void Setup()
        {
            defaultSettingsConfig = new RateLimitSettingsConfig();

            defaultSettingsConfig[RateLimitType.RequestsPerTimespan] = new TokenBucketSettings()
            {
                MaxAmount = 5,
                RefillAmount = 5,
                RefillTime = 60
            };

            defaultSettingsConfig[RateLimitType.TimespanPassedSinceLastCall] = new TimespanPassedSinceLastCallSettings()
            {
                TimespanLimit = new TimeSpan(0, 1, 0)
            };
        }

        [Test]
        public void Verify_True_RequestsPerTimespanPassAndTimespanPassedSinceLastCallPass()
        {
            // arrange 
            var clientToken = "abc123";
            var requestDate = new DateTime(2020, 1, 1, 0, 0, 1);   // 1/1/2020 12:01AM
            var lastUpdateDate = new DateTime(2020, 1, 1, 0, 0, 0);   // 1/1/2020 12:00:00AM
            var lastClientRequest = new ClientRequestData(0, lastUpdateDate);

            var fakeRepository = Substitute.For<IClientRepository>();
            fakeRepository.GetClientData(clientToken).Returns(lastClientRequest);

            var fakeRateLimiterAlgorithm = Substitute.For<IRateLimiterAlgorithm>();
            fakeRateLimiterAlgorithm.VerifyRequestsPerTimeSpan(0, 5, 5, 60, requestDate, requestDate).ReturnsForAnyArgs(true);
            fakeRateLimiterAlgorithm.VerifyTimespanPassedSinceLastCall(requestDate, new TimeSpan(0, 1, 0), requestDate).ReturnsForAnyArgs(true);

            var rateLimiter = new RateLimiter(fakeRepository, fakeRateLimiterAlgorithm);

            // act
            var result = rateLimiter.Verify(clientToken, requestDate, defaultSettingsConfig);

            // assert
            Assert.AreEqual(result, true);
        }

        [Test]
        public void Verify_False_RequestsPerTimespanFailAndTimespanPassedSinceLastCallPass()
        {
            // arrange 
            var clientToken = "abc123";
            var requestDate = new DateTime(2020, 1, 1, 0, 1, 0);   // 1/1/2020 12:01AM
            var lastUpdateDate = new DateTime(2020, 1, 1, 0, 0, 0);   // 1/1/2020 12:00AM
            var lastClientRequest = new ClientRequestData(0, lastUpdateDate);

            var fakeRepository = Substitute.For<IClientRepository>();
            fakeRepository.GetClientData(clientToken).Returns(lastClientRequest);

            var fakeRateLimiterAlgorithm = Substitute.For<IRateLimiterAlgorithm>();
            fakeRateLimiterAlgorithm.VerifyRequestsPerTimeSpan(0, 5, 5, 60, requestDate, requestDate).ReturnsForAnyArgs(false);
            fakeRateLimiterAlgorithm.VerifyTimespanPassedSinceLastCall(requestDate, new TimeSpan(0, 1, 0), requestDate).ReturnsForAnyArgs(true);

            var rateLimiter = new RateLimiter(fakeRepository, fakeRateLimiterAlgorithm);

            // act
            var result = rateLimiter.Verify(clientToken, requestDate, defaultSettingsConfig);

            // assert
            Assert.AreEqual(result, false);
        }
    }
}
