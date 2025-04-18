using System;
using NUnit.Framework;
using RateLimiter.Core;
using RateLimiter.Core.Rules;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public void TestNoRulesConfigured()
        {
            var rateLimiter = new Core.RateLimiter();
            string clientToken = "test-client";
            string resourceId = "api/no-rule";

            // should allow all requests when no rule is configured
            for (int i = 0; i < 12; i++)
            {
                Assert.That(rateLimiter.IsRequestAllowed(clientToken, resourceId), Is.True,
                    $"Request {i+1} should be allowed when no rule is configured");
            }
        }

        [Test]
        public void TestUsesCorrectRule()
        {
            var rateLimiter = new Core.RateLimiter();
            var rule = new FixedWindowRule(2, TimeSpan.FromSeconds(10));

            rateLimiter.ConfigureResource("api/test", rule);

            string clientToken = "test-client";
            string resourceId = "api/test";

            Assert.That(rateLimiter.IsRequestAllowed(clientToken, resourceId), Is.True, "First request should be allowed");
            Assert.That(rateLimiter.IsRequestAllowed(clientToken, resourceId), Is.True, "Second request should be allowed");
            Assert.That(rateLimiter.IsRequestAllowed(clientToken, resourceId), Is.False, "Third request should be denied");
        }

        [Test]
        public void TestMultipleResources()
        {
            var rateLimiter = new Core.RateLimiter();

            // configure different rules for different resources
            rateLimiter.ConfigureResource("api/resource1", new FixedWindowRule(1, TimeSpan.FromSeconds(10)));
            rateLimiter.ConfigureResource("api/resource2", new FixedWindowRule(2, TimeSpan.FromSeconds(10)));

            string clientToken = "test-client";

            // should allow 1 request
            Assert.That(rateLimiter.IsRequestAllowed(clientToken, "api/resource1"), Is.True, "First request to resource1 should be allowed");
            Assert.That(rateLimiter.IsRequestAllowed(clientToken, "api/resource1"), Is.False, "Second request to resource1 should be denied");

            // should allow 2 requests
            Assert.That(rateLimiter.IsRequestAllowed(clientToken, "api/resource2"), Is.True, "First request to resource2 should be allowed");
            Assert.That(rateLimiter.IsRequestAllowed(clientToken, "api/resource2"), Is.True, "Second request to resource2 should be allowed");
            Assert.That(rateLimiter.IsRequestAllowed(clientToken, "api/resource2"), Is.False, "Third request to resource2 should be denied");
        }
    }
}