using System;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Exceptions;

namespace RateLimiter.Tests.LimitedServiceTests
{
    [TestFixture]
    public class SampleServiceTwo
    {
        // Note: Test setup didn't work for each test - the DataStore will be cleaned up in each test separately.
        // This might be relative to the issue: https://github.com/nunit/nunit/issues/1081
        public class SampleServiceTwoX
        {
            [Test]
            public void GivenRuleBWith1MinuteTimeSpan_WhenOnly1RequestComesIn_ThenPassTheRequest()
            {
                // Arrange
                DataStore.DataStore.ClearDataStore();
                var sut = new LimitedService();

                // Act
                Action action = () =>
                {
                    sut.SampleServiceTwo("TokenA");
                };

                // Assert
                action.Should().NotThrow();
            } 
            
            [Test]
            public void GivenRuleBWith1MinuteTimeSpan_When2RequestComeIn_ThenReject()
            {
                // Arrange
                DataStore.DataStore.ClearDataStore();
                var sut = new LimitedService();

                // Act
                Action action = () =>
                {
                    sut.SampleServiceTwo("TokenA");
                    sut.SampleServiceTwo("TokenA");
                };

                // Assert
                action.Should().Throw<RequestsOutOfLimitException>();
            } 
        }
    }
}