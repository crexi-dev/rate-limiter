using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Models.Rules;
using RateLimiter.Services.Handlers;
using RateLimiter.Services.Handlers.Models;
using RateLimiter.Services.Handlers.Validators;
using RateLimiter.UtilityServices;

namespace RateLimiter.Tests.Services.Handlers
{
    [TestFixture]
    public class CertainTimespanPassedSinceTheLastCallRateLimiterRuleHandlerTests
    {
        private Mock<IBaseHandlerModelValidator> _baseHandlerModelValidator;
        private Mock<IDateTimeProvider> _dateTimeProviderMock;
        private CertainTimespanPassedSinceTheLastCallRateLimiterRuleHandler _certainTimespanPassedSinceTheLastCallRateLimiterRuleHandler;

        private Fixture _autoFixture;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _autoFixture = new Fixture();
        }

        [SetUp]
        public void SetUp()
        {
            _baseHandlerModelValidator = new Mock<IBaseHandlerModelValidator>(MockBehavior.Strict);
            _dateTimeProviderMock = new Mock<IDateTimeProvider>(MockBehavior.Strict);

            _certainTimespanPassedSinceTheLastCallRateLimiterRuleHandler = new CertainTimespanPassedSinceTheLastCallRateLimiterRuleHandler(
                _baseHandlerModelValidator.Object, _dateTimeProviderMock.Object);
        }

        [Test]
        public async Task Handle_WhenRuleIsPassed_ReturnTrue()
        {
            // Arrange
            var token = _autoFixture.Create<string>();
            var dateTimeNow = _autoFixture.Create<DateTime>();
            var lastUserEntry = dateTimeNow - TimeSpan.FromMinutes(5);
            var userInformation = _autoFixture
                .Build<UserInformation>()
                .With(x => x.Token, token)
                .With(x => x.RequestEntries, new List<DateTime> { lastUserEntry })
                .Create();
            var certainTimespanPassedSinceTheLastCallRateLimiterRule = _autoFixture
                .Build<CertainTimespanPassedSinceTheLastCallRateLimiterRule>()
                .With(x => x.TimeSpanPeriod, TimeSpan.FromSeconds(4))
                .Create();
            var certainTimespanPassedSinceTheLastCallRateLimiterRuleHandlerModel = _autoFixture
                .Build<CertainTimespanPassedSinceTheLastCallRateLimiterRuleHandlerModel>()
                .With(x => x.Token, token)
                .With(x => x.UserInformation, userInformation)
                .With(x => x.Rule, certainTimespanPassedSinceTheLastCallRateLimiterRule)
                .Create();

            _dateTimeProviderMock.Setup(x => x.GetDateTimeUtcNow()).Returns(dateTimeNow);
            _baseHandlerModelValidator.Setup(x => x.Validate(certainTimespanPassedSinceTheLastCallRateLimiterRuleHandlerModel));

            // Act
            var result = await _certainTimespanPassedSinceTheLastCallRateLimiterRuleHandler.Handle(
                certainTimespanPassedSinceTheLastCallRateLimiterRuleHandlerModel, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
            _baseHandlerModelValidator.VerifyAll();
            _dateTimeProviderMock.VerifyAll();
        }

        [Test]
        public async Task Handle_WhenRuleIsNotPassed_ReturnFalse()
        {
            // Arrange
            var token = _autoFixture.Create<string>();
            var dateTimeNow = _autoFixture.Create<DateTime>();
            var lastUserEntry = dateTimeNow - TimeSpan.FromMinutes(5);
            var userInformation = _autoFixture
                .Build<UserInformation>()
                .With(x => x.Token, token)
                .With(x => x.RequestEntries, new List<DateTime> { lastUserEntry })
                .Create();
            var certainTimespanPassedSinceTheLastCallRateLimiterRule = _autoFixture
                .Build<CertainTimespanPassedSinceTheLastCallRateLimiterRule>()
                .With(x => x.TimeSpanPeriod, TimeSpan.FromMinutes(6))
                .Create();
            var certainTimespanPassedSinceTheLastCallRateLimiterRuleHandlerModel = _autoFixture
                .Build<CertainTimespanPassedSinceTheLastCallRateLimiterRuleHandlerModel>()
                .With(x => x.Token, token)
                .With(x => x.UserInformation, userInformation)
                .With(x => x.Rule, certainTimespanPassedSinceTheLastCallRateLimiterRule)
                .Create();

            _dateTimeProviderMock.Setup(x => x.GetDateTimeUtcNow()).Returns(dateTimeNow);
            _baseHandlerModelValidator.Setup(x => x.Validate(certainTimespanPassedSinceTheLastCallRateLimiterRuleHandlerModel));

            // Act
            var result = await _certainTimespanPassedSinceTheLastCallRateLimiterRuleHandler.Handle(
                certainTimespanPassedSinceTheLastCallRateLimiterRuleHandlerModel, CancellationToken.None);

            // Assert
            Assert.IsFalse(result);
            _baseHandlerModelValidator.VerifyAll();
            _dateTimeProviderMock.VerifyAll();
        }
    }
}
