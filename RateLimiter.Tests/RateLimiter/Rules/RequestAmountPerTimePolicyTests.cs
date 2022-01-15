using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Domain;
using RateLimiter.RateLimiter.Rules;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests.RateLimiter.Rules
{
    [TestFixture]
    public sealed class RequestAmountPerTimePolicyTests
    {
        private IRateLimitPolicy underTest;

        private const int Amount = 2;
        private TimeSpan Time = TimeSpan.FromSeconds(10);

        [SetUp]
        public void SetUp() => underTest = new RequestAmountPerTimePolicy(Amount, Time);

        [Test]
        public void Check_When_Amount_Of_Requests_Exceeded_Limits_Should_Return_False()
        {
            // arrange
            var token = "token";
            var currentDate = new DateTime(2000, 12, 20, 20, 20, 50);
            var userRequests = new List<UserRequest>
            {
                new UserRequest { AccessToken = token, Date = currentDate.AddSeconds(-2) },
                new UserRequest { AccessToken = token, Date = currentDate.AddSeconds(-4) },
                new UserRequest { AccessToken = token, Date = currentDate.AddSeconds(-9) }
            };

            // act
            var result = underTest.Check(token, userRequests, currentDate);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void Check_When_Amount_Of_Requests_Is_Below_Limits_Should_Return_True()
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
