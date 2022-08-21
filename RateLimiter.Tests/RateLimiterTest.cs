using System;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Exceptions;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public void GivenRuleAWith5RequestsPerMinute_When5RequestsSentIn1Minute_ThenPassAllRequests()
        {
            // Arrange
            var sut = new LimitedService();

            // Act
            Action action = () =>
            {
                sut.SampleServiceOne("TokenA");
                sut.SampleServiceOne("TokenA");
                sut.SampleServiceOne("TokenA");
                sut.SampleServiceOne("TokenA");
                sut.SampleServiceOne("TokenA");
            };

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void GivenRuleAWith5RequestsPerMinute_When6RequestsSentIn1Minute_ThenThrowException()
        {
            // Arrange
            var sut = new LimitedService();

            // Act
            Action action = () =>
            {
                sut.SampleServiceOne("TokenA");
                sut.SampleServiceOne("TokenA");
                sut.SampleServiceOne("TokenA");
                sut.SampleServiceOne("TokenA");
                sut.SampleServiceOne("TokenA");
                sut.SampleServiceOne("TokenA");
            };

            // Assert
            action.Should().NotThrow<RequestsOutOfLimitException>();
        }
    }
}