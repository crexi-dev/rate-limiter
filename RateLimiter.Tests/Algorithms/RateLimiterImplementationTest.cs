using System;
using RateLimiter.Library;
using RateLimiter.Library.Algorithms;
using RateLimiter.Client;
using NUnit.Framework;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterImplementationTest
    {
        [Test]
        public void TokenBucketRateLimiter_Verify_True_WithinRatelimit()
        {
            var clientToken = "abc123";
            var lastUpdateDate = new DateTime(2020, 1, 1, 0, 0, 0, 0);   // 1/1/2020 12:00:00AM
            var requestDate = new DateTime(2020, 1, 1, 0, 0, 0, 500);   // 1/1/2020 12:00:05AM

            var tokenBucketRuleRateLimiter = new TokenBucketRateLimiter();
            var isAllowed = tokenBucketRuleRateLimiter.Verify(5, 5, 1, 60, requestDate, lastUpdateDate);

            Assert.AreEqual(isAllowed, true);
        }

        [Test]
        public void TimespanPassedSinceLastCall_Verify_True_WithinRatelimit()
        {
            var clientToken = "abc123";
            var lastUpdateDate = new DateTime(2020, 1, 1, 0, 0, 0, 0);   // 1/1/2020 12:00:00AM
            var requestDate = new DateTime(2020, 1, 1, 0, 0, 0, 500);   // 1/1/2020 12:00:05AM

            var timespanPassedSinceLastCallRateLimiter = new TimespanPassedRateLimiter();

            var isAllowed = timespanPassedSinceLastCallRateLimiter.Verify(requestDate, new TimeSpan(0, 0, 1), lastUpdateDate);

            Assert.AreEqual(isAllowed, true);
        }
    }
}
