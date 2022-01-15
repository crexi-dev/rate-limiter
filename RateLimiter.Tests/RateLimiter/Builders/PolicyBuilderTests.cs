using FluentAssertions;
using NUnit.Framework;
using RateLimiter.RateLimiter.Builders;
using RateLimiter.RateLimiter.Rules;
using System;

namespace RateLimiter.Tests.RateLimiter.Builders
{
    [TestFixture]   
    public sealed class PolicyBuilderTests
    {
        [Test]
        public void PolicyBuilder_When_Build_With_Policies_Should_Return_Expected_CompositePolicy()
        {
            // arrange
            var requestAmountPerTimePolicy = new RequestAmountPerTimePolicy(10, TimeSpan.FromSeconds(1000));
            var requestTimeAfterLastPolicy = new RequestTimeAfterLastPolicy(TimeSpan.FromSeconds(2000));

            var expected = new CompositePolicy(requestAmountPerTimePolicy, requestTimeAfterLastPolicy);

            // act
            var policy = new PolicyBuilder()
                .AddPolicy(requestTimeAfterLastPolicy)
                .AddPolicy(requestTimeAfterLastPolicy)
                .Build() as CompositePolicy;

            // assert
            expected.Should().BeEquivalentTo(policy, cfg => cfg.IncludingInternalFields());
        }
    }
}
