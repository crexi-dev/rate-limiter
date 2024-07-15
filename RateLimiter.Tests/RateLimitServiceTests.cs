using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Data;
using RateLimiter.Model;
using RateLimiter.Rule;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimitServiceTests
{
    private readonly IRequestTrafficDataAccess fakeRequestTrafficDataAccess;
    private readonly RateLimitService rateLimitService;

    private readonly Uri testUri;
    private readonly string testToken;

    private DateTime timestamp;

    /// <summary>
    ///     Can use 1 time setup but even XUnit doesn't even need it.
    ///     Just take simple constructor approach.
    /// </summary>
    public RateLimitServiceTests()
	{
        this.fakeRequestTrafficDataAccess = A.Fake<IRequestTrafficDataAccess>();
        this.rateLimitService = new RateLimitService(fakeRequestTrafficDataAccess);

        this.testUri = new Uri("https://www.test.com/test");
        this.testToken = "TestToken";
    }

    [SetUp]
    public void SetUp()
    {
        timestamp = DateTime.UtcNow;
    }

    [TearDown]
    public void TearDown()
    {
        this.rateLimitService.ClearRules(this.testUri);
        Fake.ClearRecordedCalls(this.fakeRequestTrafficDataAccess);
    }

    [TestCase(5, 5, 1, true)]
    [TestCase(2, 5, 1, false)]
    [TestCase(5, 5, 2, false)]
    [TestCase(2, 5, 2, false)]
    public void ACallToRequestAllow_WithMultipleRule_ShouldReturnExpectedResult(
        int maxCountPerPeriod, int samplePeriodInSeconds, int minimumDurationRequestGap, bool expectedResult)
	{
        A.CallTo(() => fakeRequestTrafficDataAccess.GetRequests(testToken, testUri))
            .ReturnsLazily(() => new List<RateLimitRequest>
            {
                new RateLimitRequest { Timestamp = DateTime.UtcNow.AddSeconds(-3), Token = testToken, Url = testUri },
                new RateLimitRequest { Timestamp = DateTime.UtcNow.AddSeconds(-2), Token = testToken, Url = testUri },
                new RateLimitRequest { Timestamp = DateTime.UtcNow.AddSeconds(-1), Token = testToken, Url = testUri },
            });
        
        this.rateLimitService.AddRule(
            testUri, 
            new MaxRequestsPerPeriodRule(maxCountPerPeriod, TimeSpan.FromSeconds(samplePeriodInSeconds)));

        this.rateLimitService.AddRule(
            testUri,
            new MinimumBetweenRequestWindowRule(TimeSpan.FromSeconds(minimumDurationRequestGap)));

        var request = new RateLimitRequest
        {
            Timestamp = timestamp,
            Token = this.testToken,
            Url = testUri
        };

        this.rateLimitService.RequestAllow(request)
            .Should().Be(expectedResult);        
	}

    [Test] 
    public void ACallToRequestAllow_WithTooManyRequestsOfDifferentTokenRequestsOnSameResource_ShouldReturnTrue()
    {
        this.rateLimitService.AddRule(
            testUri,
            new MinimumBetweenRequestWindowRule(TimeSpan.FromSeconds(2)));

        A.CallTo(() => fakeRequestTrafficDataAccess.GetRequests(testToken, testUri))
            .ReturnsLazily(() => new List<RateLimitRequest>
            {
                new RateLimitRequest { Timestamp = DateTime.UtcNow.AddSeconds(-1), Token = testToken, Url = testUri }
            });

        var request = new RateLimitRequest
        {
            Timestamp = timestamp,
            Token = "OtherToken",
            Url = testUri
        };

        this.rateLimitService.RequestAllow(request)
            .Should().BeTrue();
    }

    [Test]
    public void ACallToRequestAllow_WithTooManySameTokenRequestsOnDifferentResources_ShouldReturnTrue()
    {
        this.rateLimitService.AddRule(
            testUri,
            new MinimumBetweenRequestWindowRule(TimeSpan.FromSeconds(2)));

        A.CallTo(() => fakeRequestTrafficDataAccess.GetRequests(testToken, testUri))
            .ReturnsLazily(() => new List<RateLimitRequest>
            {
                new RateLimitRequest { Timestamp = DateTime.UtcNow.AddSeconds(-1), Token = testToken, Url = testUri }
            });

        var request = new RateLimitRequest
        {
            Timestamp = timestamp,
            Token = this.testToken,
            Url = new Uri("https://www.test.com/other-resource")
        };

        this.rateLimitService.RequestAllow(request)
            .Should().BeTrue();
    }
}