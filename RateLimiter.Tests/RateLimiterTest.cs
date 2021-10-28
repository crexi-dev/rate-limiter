using Moq;
using NUnit.Framework;
using RateLimiter.ApiRule;
using RateLimiter.ApiRule.Factory;
using RateLimiter.ApiRule.RuleImplementations;
using RateLimiter.Model.Enum;
using RateLimiter.Model.Request;
using RateLimiter.Validation;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private RequestValidator _validator;
        private Mock<IRateLimitedFactory> _mockFactory;
        private Mock<IRuleValidation> _mockRuleSuccess;
        private Mock<IRuleValidation> _mockRuleFailure;
        private MaxRequestsPerTimespanRateLimitRule _rateLimitRule;
        private TimespanPassedRateLimitRule _timeSpanRateLimitRule;
        private int _maxRequestsInTimespan;

        [SetUp]
        public void SetUp()
        {
            _mockFactory = new Mock<IRateLimitedFactory>();
            _validator = new RequestValidator(_mockFactory.Object);

            _mockRuleSuccess = new Mock<IRuleValidation>();
            _mockRuleSuccess.Setup(x => x.Validate("token1", ResourceEnum.resourcea, null)).Returns(true);

            _mockRuleFailure = new Mock<IRuleValidation>();
            _mockRuleFailure.Setup(x => x.Validate("token1", ResourceEnum.resourcea, null)).Returns(false);

            _maxRequestsInTimespan = 2;

            _rateLimitRule = new MaxRequestsPerTimespanRateLimitRule();
            _timeSpanRateLimitRule = new TimespanPassedRateLimitRule();
        }


        [Test]
        public void ValidateRequest_WhenRateLimitRulesPass_ReturnsTrueAndRequestAddedToDictionary()
        {
            _mockFactory.Setup(x => x.GetRateLimitRulesByResource(ResourceEnum.resourcea)).Returns(new List<IRuleValidation> { _mockRuleSuccess.Object });

            var initialRequestCount = _validator.GetApiRequestCountByToken("token1");
            var result = _validator.ValidateRequest("token1", ResourceEnum.resourcea);
            var updatedRequestCount = _validator.GetApiRequestCountByToken("token1");

            Assert.That(result, Is.True);
            Assert.That(updatedRequestCount, Is.EqualTo(initialRequestCount + 1));
        }

        [Test]
        public void ValidateRequest_WhenRateLimitRulesFail_ReturnsFalseAndRequestNotAddedToDictionary()
        {
            _mockFactory.Setup(x => x.GetRateLimitRulesByResource(ResourceEnum.resourcea)).Returns(new List<IRuleValidation> { _mockRuleSuccess.Object });

            var initialRequestCount = _validator.GetApiRequestCountByToken("token1");
            var result = _validator.ValidateRequest("token1", ResourceEnum.resourcea);
            var updatedRequestCount = _validator.GetApiRequestCountByToken("token1");

            Assert.That(result, Is.False);
            Assert.That(updatedRequestCount, Is.EqualTo(initialRequestCount));
        }

        [Test]
        public void Validate_WhenTokenRequestLogHasLessThanMaxRequestsInTimespanAndMoreOutsideTimespan_ReturnsTrue()
        {
            var token = "token1";
            var resourceName = ResourceEnum.resourcea;
            var tokenRequestLog = new List<ApiRequest>()
            {
                new ApiRequest { ResourceName =resourceName, DateRequested = DateTime.UtcNow.AddSeconds(-120) },
                new ApiRequest { ResourceName =resourceName, DateRequested = DateTime.UtcNow.AddSeconds(-120) },
                new ApiRequest { ResourceName = resourceName, DateRequested = DateTime.UtcNow.AddSeconds(-120) },
                new ApiRequest { ResourceName =resourceName, DateRequested = DateTime.UtcNow.AddSeconds(-30) }
            };

            var result = _rateLimitRule.Validate(token, resourceName, tokenRequestLog);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Validate_WhenTokenRequestLogIsEmpty_ReturnsTrue()
        {
            var token = "token1";
            var resourceName = ResourceEnum.resourcea;
            var tokenRequestLog = new List<ApiRequest>();

            var result = _rateLimitRule.Validate(token, resourceName, tokenRequestLog);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Validate_WhenTokenLastRequestIsGreaterThanTimespanPassed_ReturnsTrue()
        {
            var token = "token1";
            var resourceName = ResourceEnum.resourcea;
            var tokenRequestLog = new List<ApiRequest>()
            {
                new ApiRequest { ResourceName = resourceName, DateRequested = DateTime.UtcNow.AddSeconds(-120) }
            };

            var result = _timeSpanRateLimitRule.Validate(token, resourceName, tokenRequestLog);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Validate_WhenTokenRequestLogIsNull_ReturnsTrue()
        {
            var token = "token1";
            var resourceName = ResourceEnum.resourcea;
            List<ApiRequest> tokenRequestLog = null;

            var result = _timeSpanRateLimitRule.Validate(token, resourceName, tokenRequestLog);

            Assert.That(result, Is.True);
        }

    }
}
