using NUnit.Framework;
using RateLimiter.Rules;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class MaxRequestsPerTimespanRateLimitRuleTests
    {
        private MaxRequestsPerTimespanRateLimitRule _rateLimitRule;
        private int _maxRequestsInTimespan;

        [SetUp]
        public void SetUp()
        {
            ConfigurationManager.AppSettings["MaxRequestsPerTimespanRateLimitRule.RequestTimespanInMinutes"] = "1";
            ConfigurationManager.AppSettings["MaxRequestsPerTimespanRateLimitRule.MaxRequestsInTimespan"] = "2";

            _maxRequestsInTimespan = int.Parse(ConfigurationManager.AppSettings["MaxRequestsPerTimespanRateLimitRule.MaxRequestsInTimespan"]);

            _rateLimitRule = new MaxRequestsPerTimespanRateLimitRule();
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
        public void Validate_WhenTokenRequestLogHasLessThanMaxRequestsInTimespan_ReturnsTrue()
        {
            var token = "me";
            var resourceName = "res";
            var tokenRequestLog = new List<ApiRequest>()
            {
                new ApiRequest { ResourceName = resourceName, DateRequested = DateTime.UtcNow.AddSeconds(-30) }
            };

            var result = _rateLimitRule.Validate(token, resourceName, tokenRequestLog);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void Validate_WhenTokenRequestLogHasGreaterThanOrEqualToMaxRequestsInTimespan_ReturnsFalse(int numberOfRequestsBeyondMax)
        {
            var token = "me";
            var resourceName = "res";
            var tokenRequestLog = new List<ApiRequest>();

            for (int i = 0; i < _maxRequestsInTimespan + numberOfRequestsBeyondMax; i++)
                tokenRequestLog.Add(new ApiRequest() { ResourceName = resourceName, DateRequested = DateTime.UtcNow.AddSeconds(-30) });

            var result = _rateLimitRule.Validate(token, resourceName, tokenRequestLog);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Validate_WhenTokenRequestLogHasLessThanMaxRequestsInTimespanAndMoreOutsideTimespan_ReturnsTrue()
        {
            var token = "me";
            var resourceName = "res";
            var tokenRequestLog = new List<ApiRequest>()
            {
                new ApiRequest { ResourceName = resourceName, DateRequested = DateTime.UtcNow.AddSeconds(-120) },
                new ApiRequest { ResourceName = resourceName, DateRequested = DateTime.UtcNow.AddSeconds(-120) },
                new ApiRequest { ResourceName = resourceName, DateRequested = DateTime.UtcNow.AddSeconds(-120) },
                new ApiRequest { ResourceName = resourceName, DateRequested = DateTime.UtcNow.AddSeconds(-30) }
            };

            var result = _rateLimitRule.Validate(token, resourceName, tokenRequestLog);

            Assert.That(result, Is.True);
        }
    }
}
