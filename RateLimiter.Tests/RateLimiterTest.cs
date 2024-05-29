using NUnit.Framework;
using RateLimiter.Rules;
using RateLimiter.Services;
using RateLimiter.Extensions;
using RateLimiter.Tests.Helpers;
using System;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
    private const string ResourceName = "resource";
    private const string UsRegion = "US";
    private const string UsToken = "US-token";
    private const string EuRegion = "EU";
    private const string EuToken = "EU-token";

    [TestCase(UsToken, ExpectedResult = true)]
    [TestCase(EuToken, ExpectedResult = true)]
    public bool IsRequestAllowed_WithinLimit_ReturnsTrue(string token)
    {
        // Arrange
        var mockDateTimeWrapper = new MockDateTimeWrapper();
        var ruleService = new RuleProvider(mockDateTimeWrapper)
            .AddRuleForResource(ResourceName, UsRegion, new TimeSinceLastCallRule(TimeSpan.FromSeconds(1), mockDateTimeWrapper))
            .AddRuleForResource(ResourceName, EuRegion, new TimeSinceLastCallRule(TimeSpan.FromSeconds(2), mockDateTimeWrapper));

        var rateLimitingService = new RateLimitingService(ruleService);

        // Act & Assert
        return rateLimitingService.IsRequestAllowed(ResourceName, token);
    }

    [Test]
    public void IsRequestAllowed_ExceedsLimit_ReturnsFalse()
    {
        // Arrange
        var mockDateTimeWrapper = new MockDateTimeWrapper();
        var ruleService = new RuleProvider(mockDateTimeWrapper)
            .AddRuleForResource(ResourceName, UsRegion, new TimeSinceLastCallRule(TimeSpan.FromSeconds(1), mockDateTimeWrapper))
            .AddRuleForResource(ResourceName, EuRegion, new TimeSinceLastCallRule(TimeSpan.FromSeconds(2), mockDateTimeWrapper));

        var rateLimitingService = new RateLimitingService(ruleService);

        // Act
        rateLimitingService.IsRequestAllowed(ResourceName, UsToken);
        mockDateTimeWrapper.UtcNow = mockDateTimeWrapper.UtcNow.AddSeconds(1);
        var isAllowed = rateLimitingService.IsRequestAllowed(ResourceName, UsToken);

        // Assert
        Assert.IsFalse(isAllowed);
    }

    [TestCase(UsToken, ExpectedResult = true)]
    [TestCase(EuToken, ExpectedResult = true)]
    public bool IsRequestAllowed_DifferentRegions_ReturnsCorrectResult(string token)
    {
        // Arrange
        var mockDateTimeWrapper = new MockDateTimeWrapper();
        var ruleService = new RuleProvider(mockDateTimeWrapper)
            .AddRuleForResource(ResourceName, UsRegion, new TimeSinceLastCallRule(TimeSpan.FromSeconds(1), mockDateTimeWrapper))
            .AddRuleForResource(ResourceName, EuRegion, new TimeSinceLastCallRule(TimeSpan.FromSeconds(2), mockDateTimeWrapper));

        var rateLimitingService = new RateLimitingService(ruleService);

        // Act
        return rateLimitingService.IsRequestAllowed(ResourceName, token);
    }

    [Test]
    public void IsRequestAllowed_WithXRequestsPerTimespanRule_ReturnsCorrectResult()
    {
        // Arrange
        var mockDateTimeWrapper = new MockDateTimeWrapper();
        var ruleService = new RuleProvider(mockDateTimeWrapper)
            .ApplyXRequestsPerTimespanRule(ResourceName, UsRegion, 10, TimeSpan.FromSeconds(1))
            .ApplyXRequestsPerTimespanRule(ResourceName, EuRegion, 5, TimeSpan.FromSeconds(2));

        var rateLimitingService = new RateLimitingService(ruleService);

        // Act & Assert
        for (int i = 0; i < 10; i++)
        {
            Assert.IsTrue(rateLimitingService.IsRequestAllowed(ResourceName, UsToken));
        }

        Assert.IsFalse(rateLimitingService.IsRequestAllowed(ResourceName, UsToken));

        mockDateTimeWrapper.UtcNow = mockDateTimeWrapper.UtcNow.AddSeconds(2);
        Assert.IsTrue(rateLimitingService.IsRequestAllowed(ResourceName, UsToken));
    }

    [Test]
    public void IsRequestAllowed_WithXRequestsPerTimespanRule_DifferentRegions_ReturnsCorrectResult()
    {
        // Arrange
        var mockDateTimeWrapper = new MockDateTimeWrapper();
        var ruleService = new RuleProvider(mockDateTimeWrapper)
            .ApplyXRequestsPerTimespanRule(ResourceName, UsRegion, 10, TimeSpan.FromSeconds(1))
            .ApplyXRequestsPerTimespanRule(ResourceName, EuRegion, 5, TimeSpan.FromSeconds(2));

        var rateLimitingService = new RateLimitingService(ruleService);

        // Act & Assert
        for (int i = 0; i < 10; i++)
        {
            Assert.IsTrue(rateLimitingService.IsRequestAllowed(ResourceName, UsToken));
        }

        Assert.IsFalse(rateLimitingService.IsRequestAllowed(ResourceName, UsToken));

        for (int i = 0; i < 5; i++)
        {
            Assert.IsTrue(rateLimitingService.IsRequestAllowed(ResourceName, EuToken));
        }

        Assert.IsFalse(rateLimitingService.IsRequestAllowed(ResourceName, EuToken));
    }

    [Test]
    public void IsRequestAllowed_UsBasedToken_AppliesXRequestsPerTimespanRule()
    {
        // Arrange
        var mockDateTimeWrapper = new MockDateTimeWrapper();
        var ruleService = new RuleProvider(mockDateTimeWrapper)
            .ApplyXRequestsPerTimespanRule(ResourceName, UsRegion, 10, TimeSpan.FromSeconds(1))
            .ApplyTimeSinceLastCallRule(ResourceName, EuRegion, TimeSpan.FromSeconds(2));

        var rateLimitingService = new RateLimitingService(ruleService);

        // Act & Assert
        for (int i = 0; i < 10; i++)
        {
            Assert.IsTrue(rateLimitingService.IsRequestAllowed(ResourceName, UsToken));
        }

        Assert.IsFalse(rateLimitingService.IsRequestAllowed(ResourceName, UsToken));

        mockDateTimeWrapper.UtcNow = mockDateTimeWrapper.UtcNow.AddSeconds(2);
        Assert.IsTrue(rateLimitingService.IsRequestAllowed(ResourceName, UsToken));
        Assert.IsTrue(rateLimitingService.IsRequestAllowed(ResourceName, EuToken));

        Assert.IsFalse(rateLimitingService.IsRequestAllowed(ResourceName, EuToken));
        mockDateTimeWrapper.UtcNow = mockDateTimeWrapper.UtcNow.AddSeconds(3);
        Assert.IsTrue(rateLimitingService.IsRequestAllowed(ResourceName, EuToken));
    }
}