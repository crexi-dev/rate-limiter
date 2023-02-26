using NUnit.Framework;
using System.Threading;
using System;
using RateLimiter.Rules;
using NUnit.Framework.Internal;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public void TestSingleRuleRateLimitForXRequestsPerTimespanRule()
        {
            var limiter = new RateLimiterValidator();
            var rule = new XRequestsPerTimespanRule(1, TimeSpan.FromSeconds(1));
            limiter.AddRule("Resource1", rule);

            Assert.True(limiter.IsRequestAllowed("Resource1", "Client1"));
            Assert.False(limiter.IsRequestAllowed("Resource1", "Client1"));

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.True(limiter.IsRequestAllowed("Resource1", "Client1"));
        }

        [Test]
        public void TestSingleRuleRateLimitForTimeSpanSinceLastCallRule()
        {
            var limiter = new RateLimiterValidator();
            var rule = new TimeSpanSinceLastCallRule(TimeSpan.FromSeconds(1));
            limiter.AddRule("Resource1", rule);

            Assert.True(limiter.IsRequestAllowed("Resource1", "Client1"));
            Assert.False(limiter.IsRequestAllowed("Resource1", "Client1"));

            Thread.Sleep(TimeSpan.FromSeconds(2));

            Assert.True(limiter.IsRequestAllowed("Resource1", "Client1"));
        }

        [Test]
        public void TestRateLimitOnDifferentResourcesForXRequestsPerTimespanRule()
        {
            var limiter = new RateLimiterValidator();
            var rule = new XRequestsPerTimespanRule(2, TimeSpan.FromSeconds(1));
            limiter.AddRule("Resource1", rule);
            limiter.AddRule("Resource2", rule);

            Assert.True(limiter.IsRequestAllowed("Resource1", "Client1"));
            Assert.True(limiter.IsRequestAllowed("Resource1", "Client1"));
            Assert.False(limiter.IsRequestAllowed("Resource1", "Client1"));

            Assert.True(limiter.IsRequestAllowed("Resource2", "Client1"));
            Assert.True(limiter.IsRequestAllowed("Resource2", "Client1"));
            Assert.False(limiter.IsRequestAllowed("Resource2", "Client1"));

            Thread.Sleep(TimeSpan.FromSeconds(2));

            Assert.True(limiter.IsRequestAllowed("Resource1", "Client1"));
            Assert.True(limiter.IsRequestAllowed("Resource2", "Client1"));
        }

        [Test]
        public void TestRateLimitOnDifferentResourcesForTimeSpanSinceLastCallRule()
        {
            // Arrange
            var limiter = new RateLimiterValidator();
            var rule = new TimeSpanSinceLastCallRule(TimeSpan.FromSeconds(1));
            limiter.AddRule("Resource1", rule);
            limiter.AddRule("Resource2", rule);

            // Act/Assert
            Assert.True(limiter.IsRequestAllowed("Resource1", "Client1"));
            Assert.True(limiter.IsRequestAllowed("Resource2", "Client1"));
            Assert.False(limiter.IsRequestAllowed("Resource1", "Client1"));
            Assert.False(limiter.IsRequestAllowed("Resource2", "Client1"));

            Thread.Sleep(TimeSpan.FromSeconds(2));

            Assert.True(limiter.IsRequestAllowed("Resource1", "Client1"));
            Assert.True(limiter.IsRequestAllowed("Resource2", "Client1"));
        }

        [Test]
        public void TestMultipleRuleRateLimit()
        {
            var validator = new RateLimiterValidator();
            validator.AddRule("Resource1", new XRequestsPerTimespanRule(1, TimeSpan.FromSeconds(1)));
            validator.AddRule("Resource1", new TimeSpanSinceLastCallRule(TimeSpan.FromSeconds(1)));

            Assert.True(validator.IsRequestAllowed("Resource1", "Client1"));
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Assert.True(validator.IsRequestAllowed("Resource1", "Client1"));
            Assert.False(validator.IsRequestAllowed("Resource1", "Client1"));
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Assert.True(validator.IsRequestAllowed("Resource1", "Client1"));
        }

        [Test]
        public void TestRateLimitOnDifferentResources()
        {
            var validator = new RateLimiterValidator();
            validator.AddRule("Resource1", new XRequestsPerTimespanRule(1, TimeSpan.FromSeconds(1)));
            validator.AddRule("Resource2", new XRequestsPerTimespanRule(2, TimeSpan.FromSeconds(1)));

            Assert.True(validator.IsRequestAllowed("Resource1", "Client1"));
            Assert.False(validator.IsRequestAllowed("Resource1", "Client1"));

            Assert.True(validator.IsRequestAllowed("Resource2", "Client1"));
            Assert.True(validator.IsRequestAllowed("Resource2", "Client1"));
            Assert.False(validator.IsRequestAllowed("Resource2", "Client1"));
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Assert.True(validator.IsRequestAllowed("Resource2", "Client1"));
        }
    }
}