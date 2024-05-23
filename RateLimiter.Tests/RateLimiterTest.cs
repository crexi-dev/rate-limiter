using System;
using NUnit.Framework;
using RateLimiter.Rules;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTests
    {
        [Test]
        public void XRequestsPerTimespanRule_AllowsRequestsWithinLimit()
        {
            var rule = new XRequestsPerTimespanRule(3, TimeSpan.FromMinutes(1));
            var rateLimiter = new RateLimiter();
            rateLimiter.AddRule("resource1", rule);

            var clientToken = "client1";
            var requestTime = DateTime.UtcNow;

            Assert.IsTrue(rateLimiter.IsRequestAllowed(clientToken, "resource1", requestTime));
            Assert.IsTrue(rateLimiter.IsRequestAllowed(clientToken, "resource1", requestTime.AddSeconds(10)));
            Assert.IsTrue(rateLimiter.IsRequestAllowed(clientToken, "resource1", requestTime.AddSeconds(20)));
            Assert.IsFalse(rateLimiter.IsRequestAllowed(clientToken, "resource1", requestTime.AddSeconds(30)));
        }

        [Test]
        public void CertainTimespanPassedRule_RestrictsRequestsBasedOnTimespan()
        {
            var rule = new CertainTimespanPassedRule(TimeSpan.FromMinutes(1));
            var rateLimiter = new RateLimiter();
            rateLimiter.AddRule("resource2", rule);

            string clientToken = "client2";
            DateTime requestTime = DateTime.UtcNow;

            Assert.IsTrue(rateLimiter.IsRequestAllowed(clientToken, "resource2", requestTime));
            Assert.IsFalse(rateLimiter.IsRequestAllowed(clientToken, "resource2", requestTime.AddSeconds(30)));
            Assert.IsTrue(rateLimiter.IsRequestAllowed(clientToken, "resource2", requestTime.AddMinutes(1)));
        }

        [Test]
        public void CombinedRules_WorkCorrectly()
        {
            var xRequestsRule = new XRequestsPerTimespanRule(2, TimeSpan.FromMinutes(1));
            var certainTimespanRule = new CertainTimespanPassedRule(TimeSpan.FromSeconds(30));
            var rateLimiter = new RateLimiter();
            rateLimiter.AddRule("resource3", xRequestsRule);
            rateLimiter.AddRule("resource3", certainTimespanRule);

            var clientToken = "client3";
            var requestTime = DateTime.UtcNow;

            Assert.IsTrue(rateLimiter.IsRequestAllowed(clientToken, "resource3", requestTime));
            Assert.IsFalse(rateLimiter.IsRequestAllowed(clientToken, "resource3", requestTime.AddSeconds(40)));
            Assert.IsFalse(rateLimiter.IsRequestAllowed(clientToken, "resource3", requestTime.AddSeconds(50)));
            Assert.IsFalse(rateLimiter.IsRequestAllowed(clientToken, "resource3", requestTime.AddMinutes(1)));
            Assert.IsFalse(rateLimiter.IsRequestAllowed(clientToken, "resource3", requestTime.AddMinutes(2)));
        }
    }
}
