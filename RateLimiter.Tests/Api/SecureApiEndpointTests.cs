using FluentAssertions;
using Moq;
using NUnit.Framework;
using RateLimiter.Api;
using RateLimiter.RateLimiter.Services;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Tests.Api
{
    [TestFixture]
    public sealed class SecureApiEndpointTests
    {
        private IApiEndpoint<int> underTest;

        private Mock<IApiEndpoint<int>> sourceApiEndpoint;
        private Mock<IRateLimitService> rateLimitService;

        private const string Token = "token";

        [SetUp]
        public void SetUp()
        {
            sourceApiEndpoint = new Mock<IApiEndpoint<int>>();
            rateLimitService = new Mock<IRateLimitService>();

            underTest = new SecureApiEndpoint<int>(
                sourceApiEndpoint.Object,
                rateLimitService.Object);
        }

        [Test]
        public async Task ActionAsync_When_Policy_Validation_Failed_Should_Throw_Exception()
        {
            // arrange
            rateLimitService
                .Setup(s => s.ValidateAsync(Token, It.IsAny<string>()))
                .ReturnsAsync(false);

            // act
            Func<Task> act = () => underTest.ActionAsync(Token);

            // assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Rate limit policy failure.");
        }

        [Test]
        public async Task ActionAsync_When_Policy_Validation_Returned_True_Should_Execute_Action()
        {
            // arrange
            rateLimitService
                .Setup(s => s.ValidateAsync(Token, It.IsAny<string>()))
                .ReturnsAsync(true);

            // act
            var result = await underTest.ActionAsync(Token);

            // assert
            sourceApiEndpoint.Verify(x => x.ActionAsync(Token), Times.Once);
        }
    }
}
