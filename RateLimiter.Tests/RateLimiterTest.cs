using System;
using Moq;
using NUnit.Framework;
using RateLimiter.Implementation.Rules;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
    [Test]
    public void IsRequestAllowed_RequestAfterMinimumInterval_ReturnsTrue()
    {
        // Arrange
        var ruleMock = new Mock<TimespanSinceLastCallRule>(TimeSpan.FromMinutes(1));

        var clientId = "testClient";
        var resource = "testResource";
        ruleMock
            .Setup(r => r.GetLastRequestTime(clientId, resource))
            .Returns(DateTime.UtcNow.AddMinutes(-25));

        // Act
        var result = ruleMock.Object.IsRequestAllowed(clientId, resource);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void IsRequestAllowed_RequestBeforeMinimumInterval_ReturnsFalse()
    {
        // Arrange
        var rule = new Mock<TimespanSinceLastCallRule>(TimeSpan.FromMinutes(1));
        var clientId = "testClient";
        var resource = "testResource";

        rule.Object.SaveRequest(clientId, resource, DateTime.UtcNow);

        // Act
        var result = rule.Object.IsRequestAllowed(clientId, resource);

        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void IsRequestAllowed_RequestsBelowMaxWithinTimespan_ReturnsTrue()
    {
        // Arrange
        var maxRequests = 5;
        var timespan = TimeSpan.FromHours(1);
        var rule = new RequestsPerTimespanRule(maxRequests, timespan);
        var clientId = "testClient";
        var resource = "testResource";

        for (int i = 0; i < maxRequests - 1; i++)
        {
            rule.SaveRequest(clientId, resource, DateTime.UtcNow.AddMinutes(-i * 256));
        }

        // Act
        var result = rule.IsRequestAllowed(clientId, resource);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void IsRequestAllowed_RequestsExceedMaxWithinTimespan_ReturnsFalse()
    {
        // Arrange
        var maxRequests = 5;
        var timespan = TimeSpan.FromHours(1);
        var rule = new RequestsPerTimespanRule(maxRequests, timespan);
        var clientId = "testClient";
        var resource = "testResource";
        
        for (int i = 0; i < maxRequests; i++)
        {
            rule.SaveRequest(clientId, resource, DateTime.UtcNow.AddMinutes(-i * 10));
        }

        // Act
        var result = rule.IsRequestAllowed(clientId, resource);

        // Assert
        Assert.IsFalse(result);
    }
}