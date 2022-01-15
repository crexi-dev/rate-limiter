using FluentAssertions;
using Moq;
using NUnit.Framework;
using RateLimiter.Common;
using RateLimiter.Domain;
using RateLimiter.RateLimiter.Rules;
using RateLimiter.RateLimiter.Services;
using RateLimiter.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Tests.RateLimiter.Services
{
    [TestFixture]
    public sealed class RateLimitServiceTests
    {
        private Mock<IUserRequestRepository> userRequestRepository;
        private Mock<IDateTimeProvider> dateTimeProvider;
        private Mock<IRateLimitPolicy> rateLimitPolicy;

        private IRateLimitService underTest;

        private const string AccessToken = "Token";
        private const string ResourceName = "Resource";

        [SetUp]
        public void SetUp()
        {
            userRequestRepository = new Mock<IUserRequestRepository>();
            dateTimeProvider = new Mock<IDateTimeProvider>();
            rateLimitPolicy = new Mock<IRateLimitPolicy>();

            underTest = new RateLimitService(
                userRequestRepository.Object,
                dateTimeProvider.Object,
                rateLimitPolicy.Object);
        }

        [TestCase(null, "resource")]
        [TestCase("", "resource")]
        [TestCase("token", "")]
        [TestCase("token", null)]
        public async Task Validate_When_Arguments_Are_Empty_Should_Return_True(
            string token, string resourceName)
        {
            // arrange
            // act
            var result = await underTest.ValidateAsync(token, resourceName);

            // assert
            result.Should().BeTrue();
        }

        [Test]
        public async Task Validate_When_RatePolicy_Check_Returns_False_Should_Return_False()
        {
            // arrange
            var userRequests = new List<UserRequest>();
            var currentDate = DateTime.Now;

            userRequestRepository
                .Setup(s => s.GetAllAsync(AccessToken))
                .ReturnsAsync(userRequests);
            dateTimeProvider
                .Setup(x => x.GetUtcDate())
                .Returns(currentDate);
            rateLimitPolicy
                .Setup(s => s.Check(AccessToken, userRequests, currentDate))
                .Returns(false);

            // act
            var result = await underTest.ValidateAsync(AccessToken, ResourceName);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        public async Task Validate_When_RatePolicy_Check_Returns_True_Should_Return_True()
        {
            // arrange
            var userRequests = new List<UserRequest>();
            var currentDate = DateTime.Now;

            userRequestRepository
                .Setup(s => s.GetAllAsync(AccessToken))
                .ReturnsAsync(userRequests);
            dateTimeProvider
                .Setup(x => x.GetUtcDate())
                .Returns(currentDate);
            rateLimitPolicy
                .Setup(s => s.Check(AccessToken, userRequests, currentDate))
                .Returns(true);

            // act
            var result = await underTest.ValidateAsync(AccessToken, ResourceName);

            // assert
            result.Should().BeTrue();
            userRequestRepository.Verify(s => s.AddOrUpdateAsync(
                It.Is<UserRequest>(r => r.AccessToken == AccessToken
                && r.ResourceName == ResourceName && r.Date == currentDate)), Times.Once);
        }
    }
}
