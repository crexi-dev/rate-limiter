using System;
using System.Collections.Generic;
using Xunit;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules;

public class RequestPerTimeSpanRuleTests
{
    [Fact]
    public void IsRequestAllowed_ShouldReturnTrue_WhenRequestLogsAreEmpty()
    {
        // Arrange
        var rule = new RequestPerTimeSpanRule(5, TimeSpan.FromMinutes(1));
        var requestLogs = new List<DateTime>();
        var timeOfRequest = DateTime.UtcNow;

        // Act
        bool result = rule.IsRequestAllowed(requestLogs, timeOfRequest);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsRequestAllowed_ShouldReturnTrue_WhenRequestCountIsBelowLimit()
    {
        // Arrange
        var rule = new RequestPerTimeSpanRule(5, TimeSpan.FromMinutes(1));
        var requestLogs = new List<DateTime>
        {
            DateTime.UtcNow.AddSeconds(-10),
            DateTime.UtcNow.AddSeconds(-20)
        };
        var timeOfRequest = DateTime.UtcNow;

        // Act
        bool result = rule.IsRequestAllowed(requestLogs, timeOfRequest);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsRequestAllowed_ShouldReturnFalse_WhenRequestCountExceedsLimit()
    {
        // Arrange
        var rule = new RequestPerTimeSpanRule(3, TimeSpan.FromMinutes(1));
        var requestLogs = new List<DateTime>
        {
            DateTime.UtcNow.AddSeconds(-10),
            DateTime.UtcNow.AddSeconds(-20),
            DateTime.UtcNow.AddSeconds(-30)
        };
        var timeOfRequest = DateTime.UtcNow;

        // Act
        bool result = rule.IsRequestAllowed(requestLogs, timeOfRequest);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsRequestAllowed_ShouldReturnTrue_WhenRequestCountIsBelowLimitWithOutdatedRequests()
    {
        // Arrange
        var rule = new RequestPerTimeSpanRule(3, TimeSpan.FromSeconds(15));
        var requestLogs = new List<DateTime>
        {
            DateTime.UtcNow.AddSeconds(-20),
            DateTime.UtcNow.AddSeconds(-10),
            DateTime.UtcNow.AddSeconds(-5)
        };
        var timeOfRequest = DateTime.UtcNow;

        // Act
        bool result = rule.IsRequestAllowed(requestLogs, timeOfRequest);

        // Assert
        Assert.True(result);
        Assert.Equal(2, requestLogs.Count);
    }
}