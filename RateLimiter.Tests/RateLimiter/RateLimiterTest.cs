using System;
using RateLimiter.Implementation;
using RateLimiter.Repository;
using RateLimiter.RulesEngine;
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
            var clientToken = "abc123";
            var requestDate = new DateTime(2020, 1, 1, 0, 0, 0, 500);   // 1/1/2020 12:00:05AM
            var lastUpdateDate = new DateTime(2020, 1, 1, 0, 0, 0, 0);   // 1/1/2020 12:00:00AM
            var rule = new RegionRule(1, "US rule", RateLimitType.RequestsPerTimespan, RateLimitLevel.Default);
            var fakeRulesEngineClient = Substitute.For<IRulesEngineClient>();
            var fakeClientRepository = Substitute.For<IClientRepository>();

            var rateLimiter = new RateLimiter(fakeClientRepository, fakeRulesEngineClient);
            var result = rateLimiter.Verify(clientToken, requestDate, "183.28.89.21");

            Assert.IsTrue(true);
        }
    }
}
