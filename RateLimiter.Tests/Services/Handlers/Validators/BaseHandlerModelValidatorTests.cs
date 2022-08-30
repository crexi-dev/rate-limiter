using System;
using AutoFixture;
using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Models.Rules;
using RateLimiter.Services.Handlers.Models;
using RateLimiter.Services.Handlers.Validators;

namespace RateLimiter.Tests.Services.Handlers.Validators
{
    [TestFixture]
    public class BaseHandlerModelValidatorTests
    {
        private BaseHandlerModelValidator _baseHandlerModelValidator;

        private Fixture _autoFixture;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _autoFixture = new Fixture();
        }

        [SetUp]
        public void SetUp()
        {
            _baseHandlerModelValidator = new BaseHandlerModelValidator();
        }

        [Test]
        public void Validate_WhenTokenIsNull_ThenThrowArgumentException()
        {
            // Arrange
            var model = _autoFixture
                .Build<XRequestsPerTimespanRateLimiterRuleHandlerModel>()
                .With(x => x.Token, (string)null)
                .Create();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _baseHandlerModelValidator.Validate(model));
            Assert.AreEqual("Token is empty.", ex.Message);
        }

        [Test]
        public void Validate_WhenTokenIsEmpty_ThenThrowArgumentException()
        {
            // Arrange
            var model = _autoFixture
                .Build<XRequestsPerTimespanRateLimiterRuleHandlerModel>()
                .With(x => x.Token, string.Empty)
                .Create();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _baseHandlerModelValidator.Validate(model));
            Assert.AreEqual("Token is empty.", ex.Message);
        }

        [Test]
        public void Validate_WhenTokenIsWhiteSpace_ThenThrowArgumentException()
        {
            // Arrange
            var model = _autoFixture
                .Build<XRequestsPerTimespanRateLimiterRuleHandlerModel>()
                .With(x => x.Token, "   ")
                .Create();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _baseHandlerModelValidator.Validate(model));
            Assert.AreEqual("Token is empty.", ex.Message);
        }

        [Test]
        public void Validate_WhenRuleIsNull_ThenThrowArgumentException()
        {
            // Arrange
            var model = _autoFixture
                .Build<XRequestsPerTimespanRateLimiterRuleHandlerModel>()
                .With(x => x.Rule, (XRequestsPerTimespanRateLimiterRule)null)
                .Create();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _baseHandlerModelValidator.Validate(model));
            Assert.AreEqual("Rule is null.", ex.Message);
        }

        [Test]
        public void Validate_WhenUserInformationIsNull_ThenThrowArgumentException()
        {
            // Arrange
            var model = _autoFixture
                .Build<XRequestsPerTimespanRateLimiterRuleHandlerModel>()
                .With(x => x.UserInformation, (UserInformation)null)
                .Create();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _baseHandlerModelValidator.Validate(model));
            Assert.AreEqual("UserInformation is null.", ex.Message);
        }
    }
}
