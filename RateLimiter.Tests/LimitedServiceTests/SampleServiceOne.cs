using System;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Exceptions;

namespace RateLimiter.Tests.LimitedServiceTests
{
    [TestFixture]
    public class SampleServiceOne
    {
        // Note: Test setup didn't work for each test - the DataStore will be cleaned up in each test separately.
        // This might be relative to the issue: https://github.com/nunit/nunit/issues/1081
        public class SampleServiceOneX
        {
            [Test]
            public void GivenRuleAWith5RequestsPerMinute_When5RequestsSentIn1Minute_ThenPassAllRequests()
            {
                // Arrange
                DataStore.DataStore.ClearDataStore();
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
            public void GivenRuleAWith5RequestsPerMinute_When5RequestsSentIn1MinuteWithDifferentTokensComeIn_ThenPassAllRequests()
            {
                // Arrange
                DataStore.DataStore.ClearDataStore();
                var sut = new LimitedService();

                // Act
                Action action = () =>
                {
                    sut.SampleServiceOne("TokenA");
                    sut.SampleServiceOne("TokenB");
                    sut.SampleServiceOne("TokenA");
                    sut.SampleServiceOne("TokenB");
                    sut.SampleServiceOne("TokenA");
                    sut.SampleServiceOne("TokenA");
                    sut.SampleServiceOne("TokenB");
                    sut.SampleServiceOne("TokenA");
                };

                // Assert
                action.Should().NotThrow();
            }

            [Test]
            public void GivenRuleAWith5RequestsPerMinute_When6RequestsSentIn1Minute_ThenThrowException()
            {
                // Arrange
                DataStore.DataStore.ClearDataStore();
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
                action.Should().Throw<RequestsOutOfLimitException>();
            }
        }
    }
}