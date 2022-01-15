using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Domain;
using RateLimiter.RateLimiter.Rules;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests.RateLimiter.Rules
{
    [TestFixture]
    public sealed class RequestTimeAfterLastPolicyTests
    {
        private IRateLimitPolicy underTest;

        private TimeSpan Time = TimeSpan.FromSeconds(10);

        [SetUp]
        public void SetUp() => underTest = new RequestTimeAfterLastPolicy(Time);

        [Test]
        public void Check_When_Current_Request_Fails_Limits_Should_Return_False()
        {
            // arrange
            var token = "token";
            var currentDate = new DateTime(2000, 12, 20, 20, 20, 50);
            var userRequests = new List<UserRequest>
            {
                new UserRequest { AccessToken = token, Date = currentDate.AddSeconds(-11) }
            };

            // act
            var result = underTest.Check(token, userRequests, currentDate);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void Check_When_Current_Request_Meet_Limits_Expectations_Should_Return_True()
        {
            // arrange
            var token = "token";
            var currentDate = new DateTime(2000, 12, 20, 20, 20, 50);
            var userRequests = new List<UserRequest>
            {
                new UserRequest { AccessToken = token, Date = currentDate.AddSeconds(-2) }
            };

            // act
            var result = underTest.Check(token, userRequests, currentDate);

            // assert
            result.Should().BeTrue();
        }
    }
}
