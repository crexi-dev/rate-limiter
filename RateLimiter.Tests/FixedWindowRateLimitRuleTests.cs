using NUnit.Framework;
using System;
using System.Threading;

namespace RateLimiter.Tests;

[TestFixture]
public class FixedWindowRateLimitRuleTests
{
    [Test]
    public void Should_AllowRequest_When_UnderLimit()
    {
        // Arrange
        var rule = new FixedWindowRateLimitRule(2, TimeSpan.FromMinutes(1));
        var token = "testToken";

        // Act
        var firstResult = rule.IsRequestAllowed(token);
        var secondResult = rule.IsRequestAllowed(token);

        // Assert
        Assert.IsTrue(firstResult, "First request should be allowed.");
        Assert.IsTrue(secondResult, "Second request should be allowed.");
    }

    [Test]
    public void Should_DenyRequest_When_OverLimit()
    {
        // Arrange
        var rule = new FixedWindowRateLimitRule(1, TimeSpan.FromMinutes(1));
        var token = "testToken";

        // Act
        rule.IsRequestAllowed(token);
        var result = rule.IsRequestAllowed(token);

        // Assert
        Assert.IsFalse(result, "Second request should be denied.");
    }

    [Test]
    public void Should_AllowRequest_When_NewWindow()
    {
        // Arrange
        var rule = new FixedWindowRateLimitRule(1, TimeSpan.FromSeconds(1));
        var token = "testToken";

        // Act
        rule.IsRequestAllowed(token);
        Thread.Sleep(1000);
        var result = rule.IsRequestAllowed(token);

        // Assert
        Assert.IsTrue(result, "Second request should be allowed in new window.");
    }

    [Test]
    public void Should_AllowRequest_When_DiffToken()
    {
        // Arrange
        var rule = new FixedWindowRateLimitRule(1, TimeSpan.FromMinutes(1));
        var token1 = "testToken1";
        var token2 = "testToken2";

        // Act
        rule.IsRequestAllowed(token1);
        var result = rule.IsRequestAllowed(token2);

        // Assert
        Assert.IsTrue(result, "Second request should be allowed.");
    }
}
