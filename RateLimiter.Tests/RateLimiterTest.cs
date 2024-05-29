using NUnit.Framework;
using RateLimiter.Rules;
using RateLimiter.Services;
using System;
using System.Threading;

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
        var ruleService = new RuleService()
                .ConfigureResource(ResourceName)
                    .ForRegion(UsRegion)
                        .AddRule(new TimeSinceLastCallRule(TimeSpan.FromSeconds(1)))
                    .ForRegion(EuRegion)
                        .AddRule(new TimeSinceLastCallRule(TimeSpan.FromSeconds(2)));
        
        var rateLimitingService = new RateLimitingService(ruleService.GetRulesForResource(ResourceName, UsToken));

        // Act
        return rateLimitingService.IsRequestAllowed(UsToken);
    }

    [Test]
    public void IsRequestAllowed_ExceedsLimit_ReturnsTrue()
    {
        // Arrange
        var ruleService = new RuleService()
                .ConfigureResource(ResourceName)
                    .ForRegion(UsRegion)
                        .AddRule(new TimeSinceLastCallRule(TimeSpan.FromSeconds(1)))
                    .ForRegion(EuRegion)
                        .AddRule(new TimeSinceLastCallRule(TimeSpan.FromSeconds(2)));

        var rateLimitingService = new RateLimitingService(ruleService.GetRulesForResource(ResourceName, UsToken));

        // Act
        var isAllowed = rateLimitingService.IsRequestAllowed(UsToken);
        Thread.Sleep(1000); // Wait for the limit to expire
        var secondAttempt = rateLimitingService.IsRequestAllowed(UsToken);

        // Assert
        Assert.IsTrue(isAllowed);
        Assert.IsTrue(secondAttempt); // Expecting the limit to have expired
    }

    [TestCase(UsToken, ExpectedResult = true)]
    [TestCase(EuToken, ExpectedResult = true)]
    public bool IsRequestAllowed_DifferentRegions_ReturnsCorrectResult(string token)
    {
        // Arrange
        var ruleService = new RuleService()
                .ConfigureResource(ResourceName)
                    .ForRegion(UsRegion)
                        .AddRule(new TimeSinceLastCallRule(TimeSpan.FromSeconds(1)))
                    .ForRegion(EuRegion)
                        .AddRule(new TimeSinceLastCallRule(TimeSpan.FromSeconds(2)));

        var rateLimitingService = new RateLimitingService(ruleService.GetRulesForResource(ResourceName, token));

        // Act
        return rateLimitingService.IsRequestAllowed(token);
    }

    [Test]
    public void IsRequestAllowed_WithXRequestsPerTimespanRule_ReturnsCorrectResult()
    {
        // Arrange
        var ruleService = new RuleService()
            .ConfigureResource(ResourceName)
                .ForRegion(UsRegion)
                    .AddRule(new XRequestsPerTimespanRule(10, TimeSpan.FromSeconds(1))) // 10 requests per second
                .ForRegion(EuRegion)
                    .AddRule(new XRequestsPerTimespanRule(5, TimeSpan.FromSeconds(2))); // 5 requests per 2 seconds

        var rateLimitingService = new RateLimitingService(ruleService.GetRulesForResource(ResourceName, UsToken));

        // Act & Assert
        for (int i = 0; i < 10; i++)
        {
            Assert.IsTrue(rateLimitingService.IsRequestAllowed(UsToken)); // Expecting the limit not to be exceeded
        }

        Assert.IsFalse(rateLimitingService.IsRequestAllowed(UsToken)); // Expecting the limit to be exceeded

        Thread.Sleep(1000); // Wait for the limit to expire

        Assert.IsTrue(rateLimitingService.IsRequestAllowed(UsToken)); // Expecting the limit to have expired
    }

    [Test]
    public void IsRequestAllowed_WithXRequestsPerTimespanRule_DifferentRegions_ReturnsCorrectResult()
    {
        // Arrange
        var ruleService = new RuleService()
             .ConfigureResource(ResourceName)
                 .ForRegion(UsRegion)
                     .AddRule(new XRequestsPerTimespanRule(10, TimeSpan.FromSeconds(1))) // 10 requests per second
                 .ForRegion(EuRegion)
                     .AddRule(new XRequestsPerTimespanRule(5, TimeSpan.FromSeconds(2))); // 5 requests per 2 seconds

        var rateLimitingServiceUs = new RateLimitingService(ruleService.GetRulesForResource(ResourceName, UsToken));
        var rateLimitingServiceEu = new RateLimitingService(ruleService.GetRulesForResource(ResourceName, EuToken));

        // Act & Assert
        for (int i = 0; i < 10; i++)
        {
            Assert.IsTrue(rateLimitingServiceUs.IsRequestAllowed(UsToken)); // Expecting the limit not to be exceeded for US region
        }

        Assert.IsFalse(rateLimitingServiceUs.IsRequestAllowed(UsToken)); // Expecting the limit to be exceeded for US region

        for (int i = 0; i < 5; i++)
        {
            Assert.IsTrue(rateLimitingServiceEu.IsRequestAllowed(EuToken)); // Expecting the limit not to be exceeded for EU region
        }

        Assert.IsFalse(rateLimitingServiceEu.IsRequestAllowed(EuToken)); // Expecting the limit to be exceeded for EU region
    }

    [Test]
    public void IsRequestAllowed_UsBasedToken_AppliesXRequestsPerTimespanRule()
    {
        // Arrange
        var ruleService = new RuleService()
            .ConfigureResource(ResourceName)
                .ForRegion(UsRegion)
                    .AddRule(new XRequestsPerTimespanRule(10, TimeSpan.FromSeconds(1))) // 10 requests per second
                .ForRegion(EuRegion)
                    .AddRule(new TimeSinceLastCallRule(TimeSpan.FromSeconds(3))); // 1 seconds since last call

        var rateLimitingServiceUs = new RateLimitingService(ruleService.GetRulesForResource(ResourceName, UsToken));
        var rateLimitingServiceEu = new RateLimitingService(ruleService.GetRulesForResource(ResourceName, EuToken));

        // Act & Assert
        for (int i = 0; i < 10; i++)
        {
            Assert.IsTrue(rateLimitingServiceUs.IsRequestAllowed(UsToken)); // Expecting the limit not to be exceeded for US-based token
        }

        Assert.IsFalse(rateLimitingServiceUs.IsRequestAllowed(UsToken)); // Expecting the limit to be exceeded for US-based token

        Thread.Sleep(1000); // Wait for the limit to expire

        Assert.IsTrue(rateLimitingServiceUs.IsRequestAllowed(UsToken)); // Expecting the limit to have expired

        Assert.IsTrue(rateLimitingServiceEu.IsRequestAllowed(EuToken)); // Expecting the rule to be applied for EU-based token
        Thread.Sleep(2000); // Wait for the rule to take effect
        Assert.IsFalse(rateLimitingServiceEu.IsRequestAllowed(EuToken)); // Expecting the rule to be applied for EU-based token
    }
}