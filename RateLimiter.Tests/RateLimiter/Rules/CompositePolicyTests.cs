using FluentAssertions;
using Moq;
using NUnit.Framework;
using RateLimiter.Domain;
using RateLimiter.RateLimiter.Rules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Tests.RateLimiter.Rules
{
    [TestFixture]
    public sealed class CompositePolicyTests
    {
        private Mock<IRateLimitPolicy> requestAmountPerTimePolicy;
        private Mock<IRateLimitPolicy> requestTimeAfterLastPolicy;

        private IRateLimitPolicy underTest;

        [SetUp]
        public void SetUp()
        {
            requestAmountPerTimePolicy = new Mock<IRateLimitPolicy>();
            requestTimeAfterLastPolicy = new Mock<IRateLimitPolicy>();

            underTest = new CompositePolicy(
                requestAmountPerTimePolicy.Object,
                requestTimeAfterLastPolicy.Object);
        }

        [TestCase(false, true, false)]
        public void Check_When_Any_Policy_Return_False_Should_Return_False(
            bool requestAmountPerTimePolicyResult, bool requestTimeAfterLastPolicyResult, bool expected)
        {
            // arrange
            requestAmountPerTimePolicy.Setup(x => x.Check(
                It.IsAny<string>(), It.IsAny<IEnumerable<UserRequest>>(), It.IsAny<DateTime>()))
                .Returns(requestAmountPerTimePolicyResult);
            requestTimeAfterLastPolicy.Setup(x => x.Check(
                It.IsAny<string>(), It.IsAny<IEnumerable<UserRequest>>(), It.IsAny<DateTime>()))
                .Returns(requestTimeAfterLastPolicyResult);

            // act
            var result = underTest.Check("token", Enumerable.Empty<UserRequest>(), DateTime.MinValue);

            // assert
            expected.Should().Be(result);
        }

    }
}
