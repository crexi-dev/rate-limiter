using System;
using System.Threading;
using NUnit.Framework;
using RateLimiter.Core.Rules;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class TokenBucketTest
    {
        [Test]
        public void TestTokenBucketAllowAndDeny()
        {
            // create a token bucket with 3 tokens and no refill
            var rule = new TokenBucketRule(3, 0);
            string clientToken = "test-client";
            string resourceId = "api/test";

            // should allow exactly 3 requests
            Assert.That(rule.IsAllowed(clientToken, resourceId), Is.True, "First request should be allowed");
            Assert.That(rule.IsAllowed(clientToken, resourceId), Is.True, "Second request should be allowed");
            Assert.That(rule.IsAllowed(clientToken, resourceId), Is.True, "Third request should be allowed");
            Assert.That(rule.IsAllowed(clientToken, resourceId), Is.False, "Fourth request should be denied");
        }

        [Test]
        public void TestTokenBucketWithRefill()
        {
            // create a token bucket with 1 token that refills at 1 token per second
            var rule = new TokenBucketRule(1, 1.0);
            string clientToken = "test-client";
            string resourceId = "api/test";

            Assert.That(rule.IsAllowed(clientToken, resourceId), Is.True, "First request should be allowed");
            Assert.That(rule.IsAllowed(clientToken, resourceId), Is.False, "Request before refill should be denied");

            // wait for token to refill
            Thread.Sleep(1100);

            Assert.That(rule.IsAllowed(clientToken, resourceId), Is.True, "Request after refill should be allowed");
        }
    }
}