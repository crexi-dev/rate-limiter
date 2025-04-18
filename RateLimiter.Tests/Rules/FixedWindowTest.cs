using System;
using System.Threading;
using NUnit.Framework;
using RateLimiter.Core.Rules;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class FixedWindowTest
    {
        [Test]
        public void TestAllowAndDeny()
        {
            // create a fixed window rule with max 3 requests per 10-second window
            var rule = new FixedWindowRule(3, TimeSpan.FromSeconds(10));
            string clientToken = "test-client";
            string resourceId = "api/test";

            // should allow exactly 3 requests
            Assert.That(rule.IsAllowed(clientToken, resourceId), Is.True, "First request should be allowed");
            Assert.That(rule.IsAllowed(clientToken, resourceId), Is.True, "Second request should be allowed");
            Assert.That(rule.IsAllowed(clientToken, resourceId), Is.True, "Third request should be allowed");
            Assert.That(rule.IsAllowed(clientToken, resourceId), Is.False, "Fourth request should be denied");
        }

        [Test]
        public void TestWindowReset()
        {
            // create a fixed window rule with max 1 request per 1 sec window
            var rule = new FixedWindowRule(1, TimeSpan.FromSeconds(1));
            string clientToken = "test-client";
            string resourceId = "api/test";

            Assert.That(rule.IsAllowed(clientToken, resourceId), Is.True, "First request should be allowed");
            Assert.That(rule.IsAllowed(clientToken, resourceId), Is.False, "Second request in same window should be denied");

            // wait for token to reset
            Thread.Sleep(1100);

            Assert.That(rule.IsAllowed(clientToken, resourceId), Is.True, "Request in new window should be allowed");
        }
    }
}