using Moq;
using NUnit.Framework;
using RateLimiter.Rules;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class ApiRequestValidatorTests
    {
        private ApiRequestValidator _validator;
        private Mock<IRateLimitRuleFactory> _mockFactory;
        private Mock<IRateLimitRule> _mockRuleSuccess;
        private Mock<IRateLimitRule> _mockRuleFailure;

        [SetUp]
        public void SetUp()
        {
            _mockFactory = new Mock<IRateLimitRuleFactory>();
            _validator = new ApiRequestValidator(_mockFactory.Object);

            _mockRuleSuccess = new Mock<IRateLimitRule>();
            _mockRuleSuccess.Setup(x => x.Validate("me", "res", null)).Returns(true);

            _mockRuleFailure = new Mock<IRateLimitRule>();
            _mockRuleFailure.Setup(x => x.Validate("me", "res", null)).Returns(false);
        }

        [Test]
        public void ValidateRequest_WhenRateLimitRulesPass_ReturnsTrueAndRequestAddedToDictionary()
        {
            _mockFactory.Setup(x => x.GetRateLimitRulesByResource("res")).Returns(new List<IRateLimitRule> { _mockRuleSuccess.Object });

            var initialRequestCount = _validator.GetApiRequestCountByToken("me");
            var result = _validator.ValidateRequest("me", "res");
            var updatedRequestCount = _validator.GetApiRequestCountByToken("me");

            Assert.That(result, Is.True);
            Assert.That(updatedRequestCount, Is.EqualTo(initialRequestCount + 1));
        }

        [Test]
        public void ValidateRequest_WhenRateLimitRulesFail_ReturnsFalseAndRequestNotAddedToDictionary()
        {
            _mockFactory.Setup(x => x.GetRateLimitRulesByResource("res")).Returns(new List<IRateLimitRule> { _mockRuleFailure.Object });

            var initialRequestCount = _validator.GetApiRequestCountByToken("me");
            var result = _validator.ValidateRequest("me", "res");
            var updatedRequestCount = _validator.GetApiRequestCountByToken("me");

            Assert.That(result, Is.False);
            Assert.That(updatedRequestCount, Is.EqualTo(initialRequestCount));
        }

        [Test]
        public void ValidateRequest_WhenRateLimitRulesPassAndFail_ReturnsFalseAndRequestNotAddedToDictionary()
        {
            _mockFactory.Setup(x => x.GetRateLimitRulesByResource("res")).Returns(new List<IRateLimitRule> { _mockRuleSuccess.Object, _mockRuleFailure.Object });

            var initialRequestCount = _validator.GetApiRequestCountByToken("me");
            var result = _validator.ValidateRequest("me", "res");
            var updatedRequestCount = _validator.GetApiRequestCountByToken("me");

            Assert.That(result, Is.False);
            Assert.That(updatedRequestCount, Is.EqualTo(initialRequestCount));
        }
    }
}
