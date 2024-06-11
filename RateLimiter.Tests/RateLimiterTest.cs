using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NUnit.Framework;
using RateLimiter.Rules;
using System;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
	[Test]
	public void FixedWindowLimiterRule_should_not_allow_excessive_requests()
	{
        var fakeTime = new FakeTimeProvider(startDateTime: DateTimeOffset.UtcNow);
        var rateLimiter = new RateLimiter();
		rateLimiter.AddRule("api", new FixedWindowLimiterRule(fakeTime).Configure(new FixedWindowLimiterRuleConfiguration(Window: TimeSpan.FromSeconds(5), Limit: 2)));

		rateLimiter.IsRequestAllowed("api", "user1").Should().BeTrue();
        rateLimiter.IsRequestAllowed("api", "user1").Should().BeTrue();
        rateLimiter.IsRequestAllowed("api", "user1").Should().BeFalse();

        fakeTime.Advance(TimeSpan.FromSeconds(6));

        rateLimiter.IsRequestAllowed("api", "user1").Should().BeTrue();
    }

    [Test]
    public void FixedWindowLimiterRule_should_have_separate_limits_for_different_users()
    {
        var fakeTime = new FakeTimeProvider(startDateTime: DateTimeOffset.UtcNow);
        var rateLimiter = new RateLimiter();
        rateLimiter.AddRule("api", new FixedWindowLimiterRule(fakeTime).Configure(new FixedWindowLimiterRuleConfiguration(Window: TimeSpan.FromSeconds(5), Limit: 1)));

        rateLimiter.IsRequestAllowed("api", "user1").Should().BeTrue();
        rateLimiter.IsRequestAllowed("api", "user1").Should().BeFalse();

        rateLimiter.IsRequestAllowed("api", "user2").Should().BeTrue();
        rateLimiter.IsRequestAllowed("api", "user2").Should().BeFalse();

    }
}