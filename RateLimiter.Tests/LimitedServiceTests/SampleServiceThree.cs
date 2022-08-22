using System;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Exceptions;

namespace RateLimiter.Tests.LimitedServiceTests
{
    [TestFixture]
    public class SampleServiceThree
    {
        // Note: Test setup didn't work for each test - the DataStore will be cleaned up in each test separately.
        // This might be relative to the issue: https://github.com/nunit/nunit/issues/1081
        [Test]
        public void
            Given3RequestsIn10SecondsForUsBasedAnd1RequestIn5SecondsForEuBased_WhenInAllowedRange_ThenPassAllRequests()
        {
            // Arrange
            DataStore.DataStore.ClearDataStore();
            var sut = new LimitedService();

            // Act
            Action action = () =>
            {
                sut.SampleServiceThree("US-TokenA");
                sut.SampleServiceThree("US-TokenA");
                sut.SampleServiceThree("US-TokenA");
                sut.SampleServiceThree("EU-TokenA");
            };

            // Assert
            action.Should().NotThrow();
        }
        
        [Test]
        public void
            Given4RequestsIn10SecondsForUsBased_WhenUsRequestsAreNotInAllowedRange_ThenReject()
        {
            // Arrange
            DataStore.DataStore.ClearDataStore();
            var sut = new LimitedService();

            // Act
            Action action = () =>
            {
                sut.SampleServiceThree("US-TokenA");
                sut.SampleServiceThree("US-TokenA");
                sut.SampleServiceThree("US-TokenA");
                sut.SampleServiceThree("US-TokenA");
            };

            // Assert
            action.Should().Throw<RequestsOutOfLimitException>();
        }
        
        [Test]
        public void
            Given4RequestsIn10SecondsForUsBasedAnd1RequestIn5SecondsForEuBased_WhenUsIsNotInRangeAndEuIsInRange_ThenRejectUsAndPassEu()
        {
            // Arrange
            DataStore.DataStore.ClearDataStore();
            var sut = new LimitedService();

            // Act
            Action usAction = () =>
            {
                sut.SampleServiceThree("US-TokenA");
                sut.SampleServiceThree("US-TokenA");
                sut.SampleServiceThree("US-TokenA");
                sut.SampleServiceThree("US-TokenA");
            };

            Action euAction = () =>
            {
                sut.SampleServiceThree("EU-TokenA");
            };

            // Assert
            usAction.Should().Throw<RequestsOutOfLimitException>();
            euAction.Should().NotThrow();
        }
    }
}