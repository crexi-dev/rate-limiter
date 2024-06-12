using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NUnit.Framework;
using RateLimiter.Rules;
using System;

namespace RateLimiter.Tests;
public partial class RateLimiterTest
{

    [Test]
    public void SingleRequestPerWindowLimiterRule_should_allow_only_one_request_per_timespan()
    {
        var fakeTime = new FakeTimeProvider(startDateTime: DateTimeOffset.UtcNow);
        var rateLimiter = new RateLimiter();
        rateLimiter.AddRule("api", new SingleRequestPerWindowLimiterRule(fakeTime).Configure(TimeSpan.FromSeconds(5)));

        rateLimiter.IsRequestAllowed("api", "user1").Should().BeTrue();
        rateLimiter.IsRequestAllowed("api", "user1").Should().BeFalse();

        rateLimiter.IsRequestAllowed("api", "user2").Should().BeTrue();
        rateLimiter.IsRequestAllowed("api", "user2").Should().BeFalse();
    }
}
