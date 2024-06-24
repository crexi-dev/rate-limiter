using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Tests;

[TestFixture]
public class TimeSinceLastCallRuleTests
{
    [Test]
    public void Should_DenyRequest_When_OverInterval()
    {
        // Arrange
        var rule = new TimeSinceLastCallRule(TimeSpan.FromSeconds(5));
        var token = "testToken";

        // Act
        rule.IsRequestAllowed(token);
        var result = rule.IsRequestAllowed(token);

        // Assert
        Assert.IsFalse(result, "Second request should be denied.");
    }

    [Test]
    public void Should_AllowRequest_When_NewInterval()
    {
        // Arrange
        var rule = new TimeSinceLastCallRule(TimeSpan.FromSeconds(1));
        var token = "testToken";

        // Act
        rule.IsRequestAllowed(token);
        Task.Delay(1000).Wait();
        var result = rule.IsRequestAllowed(token);

        // Assert
        Assert.IsTrue(result, "Second request should be allowed in new interval.");
    }

    [Test]
    public void Should_AllowRequest_When_DiffToken()
    {
        // Arrange
        var rule = new TimeSinceLastCallRule(TimeSpan.FromSeconds(1));
        var token1 = "testToken1";
        var token2 = "testToken2";

        // Act
        rule.IsRequestAllowed(token1);
        var result = rule.IsRequestAllowed(token2);

        // Assert
        Assert.IsTrue(result, "Second request should be allowed.");
    }
}
