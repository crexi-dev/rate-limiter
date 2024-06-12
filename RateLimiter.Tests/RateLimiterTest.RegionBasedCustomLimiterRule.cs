using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NUnit.Framework;
using RateLimiter.Rules;
using System;

namespace RateLimiter.Tests;
public partial class RateLimiterTest
{

    [Test]
    public void RegionBasedCustomLimiterRule_should_support_own_rule_for_each_region()
    {
        var fakeTime = new FakeTimeProvider(startDateTime: DateTimeOffset.UtcNow);
        var rateLimiter = new RateLimiter();
        rateLimiter.AddRule("api", new RegionBasedCustomLimiterRule().Configure(
            euRule: new SingleRequestPerWindowLimiterRule(fakeTime).Configure(TimeSpan.FromSeconds(2)),
            usRule: new FixedWindowLimiterRule(fakeTime).Configure(new FixedWindowLimiterRuleConfiguration(Window: TimeSpan.FromSeconds(5), Limit: 2))));

        var euUserToken = GetRegionSpecificToken(Region.EU, "user1");
        var usUserToken = GetRegionSpecificToken(Region.US, "user2");

        rateLimiter.IsRequestAllowed("api", euUserToken).Should().BeTrue();
        rateLimiter.IsRequestAllowed("api", euUserToken).Should().BeFalse();

        rateLimiter.IsRequestAllowed("api", usUserToken).Should().BeTrue();
        rateLimiter.IsRequestAllowed("api", usUserToken).Should().BeTrue();
    }

    [Test]
    public void RegionBasedCustomLimiterRule_shouldnt_limit_users_which_region_is_unable_to_determine()
    {
        var fakeTime = new FakeTimeProvider(startDateTime: DateTimeOffset.UtcNow);
        var rateLimiter = new RateLimiter();
        rateLimiter.AddRule("api", new RegionBasedCustomLimiterRule().Configure(
            euRule: new SingleRequestPerWindowLimiterRule(fakeTime).Configure(TimeSpan.FromSeconds(2)),
            usRule: new FixedWindowLimiterRule(fakeTime).Configure(new FixedWindowLimiterRuleConfiguration(Window: TimeSpan.FromSeconds(5), Limit: 1))));

        var userToken = "uk:user1";

        rateLimiter.IsRequestAllowed("api", userToken).Should().BeTrue();
        rateLimiter.IsRequestAllowed("api", userToken).Should().BeTrue();
    }

    private static string GetRegionSpecificToken(Region region, string token)
    {
        return $"{region}:{token}";
    }
}
