using NUnit.Framework;
using RateLimiter.Rules;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class TimespanPassedRateLimitRuleTests
    { 
        private TimespanPassedRateLimitRule _rateLimitRule;

        [SetUp]
        public void SetUp()
        {
            ConfigurationManager.AppSettings["TimespanPassedRateLimitRule.TimespanPassedInMinutes"] = "1";

            _rateLimitRule = new TimespanPassedRateLimitRule();
        }

        [Test]
        public void Validate_WhenTokenRequestLogIsNull_ReturnsTrue()
        {
            var token = "me";
            var resourceName = "res";
            List<ApiRequest> tokenRequestLog = null;

            var result = _rateLimitRule.Validate(token, resourceName, tokenRequestLog);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Validate_WhenTokenRequestLogIsEmpty_ReturnsTrue()
        {
            var token = "me";
            var resourceName = "res";
            var tokenRequestLog = new List<ApiRequest>();

            var result = _rateLimitRule.Validate(token, resourceName, tokenRequestLog);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Validate_WhenTokenLastRequestIsGreaterThanTimespanPassed_ReturnsTrue()
        {
            var token = "me";
            var resourceName = "res";
            var tokenRequestLog = new List<ApiRequest>()
            {
                new ApiRequest { ResourceName = resourceName, DateRequested = DateTime.UtcNow.AddSeconds(-120) }
            };

            var result = _rateLimitRule.Validate(token, resourceName, tokenRequestLog);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase(-60)]
        [TestCase(-30)]
        public void Validate_WhenTokenLastRequestIsLessThanOrEqualTimespanPassed_ReturnsFalse(int secondsSinceLastRequest)
        {
            var token = "me";
            var resourceName = "res";
            var tokenRequestLog = new List<ApiRequest>()
            {
                new ApiRequest { ResourceName = resourceName, DateRequested = DateTime.UtcNow.AddSeconds(secondsSinceLastRequest) }
            };

            var result = _rateLimitRule.Validate(token, resourceName, tokenRequestLog);

            Assert.That(result, Is.False);
        }
    }
}
