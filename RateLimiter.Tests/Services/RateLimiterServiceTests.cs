using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RateLimiter.Exceptions;
using RateLimiter.InMemoryCache.Interfaces;
using RateLimiter.Models;
using RateLimiter.Models.Enums;
using RateLimiter.Models.Rules;
using RateLimiter.Options;
using RateLimiter.Services;
using RateLimiter.Services.Handlers.Models;

namespace RateLimiter.Tests.Services
{
    [TestFixture]
    public class RateLimiterServiceTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<IOptions<RateLimiterOptions>> _rateLimiterOptionsMock;
        private RateLimiterOptions _rateLimiterOptions;
        private Mock<IInMemoryCacheProxy> _inMemoryCacheProxyMock;

        private RateLimiterService _rateLimiterService;

        private Fixture _autoFixture;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _autoFixture = new Fixture();
        }

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
            _rateLimiterOptionsMock = new Mock<IOptions<RateLimiterOptions>>(MockBehavior.Strict);
            _inMemoryCacheProxyMock = new Mock<IInMemoryCacheProxy>(MockBehavior.Strict);

            _rateLimiterOptions = new RateLimiterOptions();
            _rateLimiterOptionsMock.Setup(x => x.Value).Returns(_rateLimiterOptions);

            _rateLimiterService = new RateLimiterService(_mediatorMock.Object, _rateLimiterOptionsMock.Object,
                _inMemoryCacheProxyMock.Object);
        }

        [Test]
        public async Task ValidateRateLimitsAsync_WhenExistingUserInformationIsNull_ThenNoRulesAreProcessed()
        {
            // Arrange
            var token = _autoFixture.Create<string>();
            var rateLimiterTypes = new List<RateLimiterType>
            {
                RateLimiterType.CertainTimespanPassedSinceTheLastCall,
                RateLimiterType.XRequestsPerTimespan
            };
            var userInformation = (UserInformation)null;
            _inMemoryCacheProxyMock.Setup(x => x.GetEntity<UserInformation>(token)).Returns(userInformation);
            _inMemoryCacheProxyMock.Setup(x => x.AddOrUpdateEntity(token, It.IsAny<UserInformation>()));

            // Act
            await _rateLimiterService.ValidateRateLimitsAsync(token, rateLimiterTypes);

            // Assert
            Assert.Pass();
            _mediatorMock.VerifyAll();
            _rateLimiterOptionsMock.VerifyAll();
            _inMemoryCacheProxyMock.VerifyAll();
        }

        [Test]
        public void ValidateRateLimitsAsync_WhenUserExistsAndRulesAreNotPassed_ThenThrowRateLimiterFailedException()
        {
            // Arrange
            var token = _autoFixture.Create<string>();
            var rateLimiterTypes = new List<RateLimiterType>
            {
                RateLimiterType.CertainTimespanPassedSinceTheLastCall,
                RateLimiterType.XRequestsPerTimespan
            };
            var userInformation = _autoFixture
                .Build<UserInformation>()
                .With(x => x.Token, token)
                .Create();
            _rateLimiterOptions.RateLimiterRules = new Dictionary<RateLimiterType, RateLimiterRuleBase>
                {
                    {
                        RateLimiterType.XRequestsPerTimespan, new XRequestsPerTimespanRateLimiterRule
                        {
                            RequestsLimit = 1,
                            TimeSpanPeriod = TimeSpan.FromSeconds(20)
                        }
                    },
                    {
                        RateLimiterType.CertainTimespanPassedSinceTheLastCall,
                        new CertainTimespanPassedSinceTheLastCallRateLimiterRule()
                        {
                            TimeSpanPeriod = TimeSpan.FromSeconds(20)
                        }
                    }
            };
            _inMemoryCacheProxyMock.Setup(x => x.GetEntity<UserInformation>(token)).Returns(userInformation);
            _mediatorMock.Setup(x => x.Send(It.IsAny<XRequestsPerTimespanRateLimiterRuleHandlerModel>(), CancellationToken.None))
                .ReturnsAsync(false);
            _mediatorMock.Setup(x => x.Send(It.IsAny<CertainTimespanPassedSinceTheLastCallRateLimiterRuleHandlerModel>(), CancellationToken.None))
                .ReturnsAsync(true);

            // Act & Assert
            var ex = Assert.ThrowsAsync<RateLimiterFailedException>(() => _rateLimiterService.ValidateRateLimitsAsync(token, rateLimiterTypes));

            Assert.AreEqual(ex?.Message, "Rate Limit validation is not passed.");
            _mediatorMock.VerifyAll();
            _rateLimiterOptionsMock.VerifyAll();
            _inMemoryCacheProxyMock.VerifyAll();
        }

        [Test]
        public async Task ValidateRateLimitsAsync_WhenUserExists_ThenProcessRules()
        {
            // Arrange
            var token = _autoFixture.Create<string>();
            var rateLimiterTypes = new List<RateLimiterType>
            {
                RateLimiterType.CertainTimespanPassedSinceTheLastCall,
                RateLimiterType.XRequestsPerTimespan
            };
            var userInformation = _autoFixture
                .Build<UserInformation>()
                .With(x => x.Token, token)
                .Create();
            _rateLimiterOptions.RateLimiterRules = new Dictionary<RateLimiterType, RateLimiterRuleBase>
                {
                    {
                        RateLimiterType.XRequestsPerTimespan, new XRequestsPerTimespanRateLimiterRule
                        {
                            RequestsLimit = 1,
                            TimeSpanPeriod = TimeSpan.FromSeconds(20)
                        }
                    },
                    {
                        RateLimiterType.CertainTimespanPassedSinceTheLastCall,
                        new CertainTimespanPassedSinceTheLastCallRateLimiterRule()
                        {
                            TimeSpanPeriod = TimeSpan.FromSeconds(20)
                        }
                    }
            };
            _inMemoryCacheProxyMock.Setup(x => x.GetEntity<UserInformation>(token)).Returns(userInformation);
            _mediatorMock.Setup(x => x.Send(It.IsAny<XRequestsPerTimespanRateLimiterRuleHandlerModel>(), CancellationToken.None))
                .ReturnsAsync(true);
            _mediatorMock.Setup(x => x.Send(It.IsAny<CertainTimespanPassedSinceTheLastCallRateLimiterRuleHandlerModel>(), CancellationToken.None))
                .ReturnsAsync(true);

            // Act
            await _rateLimiterService.ValidateRateLimitsAsync(token, rateLimiterTypes);

            // Assert
            Assert.Pass();
            _mediatorMock.VerifyAll();
            _rateLimiterOptionsMock.VerifyAll();
            _inMemoryCacheProxyMock.VerifyAll();
        }
    }
}
