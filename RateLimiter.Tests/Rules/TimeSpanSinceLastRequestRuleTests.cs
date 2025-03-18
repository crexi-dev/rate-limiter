using System;
using System.Collections.Generic;
using Xunit;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules;

public class TimeSpanSinceLastRequestRuleTests
{
    [Fact]
    public void IsRequestAllowed_ShouldReturnTrue_WhenRequestLogsAreEmpty()
    {
        // Arrange
        var rule = new TimeSpanSinceLastRequestRule(TimeSpan.FromSeconds(10));
        var requestLogs = new List<DateTime>();
        var timeOfRequest = DateTime.UtcNow;

        // Act
        bool result = rule.IsRequestAllowed(requestLogs, timeOfRequest);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsRequestAllowed_ShouldReturnTrue_WhenTimeSinceLastRequestExceedsLimit()
    {
        // Arrange
        var rule = new TimeSpanSinceLastRequestRule(TimeSpan.FromSeconds(10));
        var requestLogs = new List<DateTime>
        {
            DateTime.UtcNow.AddSeconds(-15)
        };
        var timeOfRequest = DateTime.UtcNow;

        // Act
        bool result = rule.IsRequestAllowed(requestLogs, timeOfRequest);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsRequestAllowed_ShouldReturnFalse_WhenTimeSinceLastRequestIsBelowLimit()
    {
        // Arrange
        var rule = new TimeSpanSinceLastRequestRule(TimeSpan.FromSeconds(10));
        var requestLogs = new List<DateTime>
        {
            DateTime.UtcNow.AddSeconds(-5)
        };
        var timeOfRequest = DateTime.UtcNow;

        // Act
        bool result = rule.IsRequestAllowed(requestLogs, timeOfRequest);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsRequestAllowed_ShouldReturnTrue_WhenLastRequestExceedsLimitWithMultipleOlderRequestsInLogs()
    {
        // Arrange
        var rule = new TimeSpanSinceLastRequestRule(TimeSpan.FromSeconds(10));
        var requestLogs = new List<DateTime>
        {
            DateTime.UtcNow.AddSeconds(-30),
            DateTime.UtcNow.AddSeconds(-25),
            DateTime.UtcNow.AddSeconds(-20)
        };
        var timeOfRequest = DateTime.UtcNow;

        // Act
        bool result = rule.IsRequestAllowed(requestLogs, timeOfRequest);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsRequestAllowed_ShouldReturnFalse_WhenLastRequestIsBelowLimitWithMultipleRequestsInLogs()
    {
        // Arrange
        var rule = new TimeSpanSinceLastRequestRule(TimeSpan.FromSeconds(10));
        var requestLogs = new List<DateTime>
        {
            DateTime.UtcNow.AddSeconds(-20),
            DateTime.UtcNow.AddSeconds(-15),
            DateTime.UtcNow.AddSeconds(-5)
        };
        var timeOfRequest = DateTime.UtcNow;

        // Act
        bool result = rule.IsRequestAllowed(requestLogs, timeOfRequest);

        // Assert
        Assert.False(result);
    }
}