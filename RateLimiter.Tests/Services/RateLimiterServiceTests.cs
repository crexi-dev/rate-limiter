using System;
using System.Collections.Generic;
using Xunit;
using RateLimiter.Rules;
using RateLimiter.Services;

namespace RateLimiter.Tests.Services;

public class RateLimiterServiceTests
{
    [Fact]
    public void IsRequestAllowed_ShouldReturnTrue_WhenNoRulesExist()
    {
        // Arrange
        var rules = new Dictionary<string, List<IRateLimitRule>>();
        var service = new RateLimiterService(rules);

        // Act
        bool result = service.IsRequestAllowed("/test/endpoint", "client-token");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsRequestAllowed_ShouldReturnTrue_WhenNoRulesExistForEndpoint()
    {
        // Arrange
        var rules = new Dictionary<string, List<IRateLimitRule>>
        {
            {
                "/test/endpoint",
                new List<IRateLimitRule>
                {
                    new RequestPerTimeSpanRule(1, TimeSpan.FromMinutes(1))
                }
            }
        };
        var service = new RateLimiterService(rules);

        // Act
        bool result = service.IsRequestAllowed("/test/endpoint-one", "client-token");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsRequestAllowed_ShouldReturnFalse_WhenRuleBlocksRequest()
    {
        // Arrange
        var rules = new Dictionary<string, List<IRateLimitRule>>
        {
            {
                "/test/endpoint",
                new List<IRateLimitRule>
                {
                    new RequestPerTimeSpanRule(1, TimeSpan.FromMinutes(1))
                }
            }
        };
        var service = new RateLimiterService(rules);

        // Act
        bool firstRequest = service.IsRequestAllowed("/test/endpoint", "client-token");
        bool secondRequest = service.IsRequestAllowed("/test/endpoint", "client-token");

        // Assert
        Assert.True(firstRequest);
        Assert.False(secondRequest);
    }

    [Fact]
    public void IsRequestAllowed_ShouldReturnTrue_WhenRequestsAreFromDifferentClients()
    {
        // Arrange
        var rules = new Dictionary<string, List<IRateLimitRule>>
        {
            {
                "/test/endpoint",
                new List<IRateLimitRule>
                {
                    new RequestPerTimeSpanRule(1, TimeSpan.FromMinutes(1))
                }
            }
        };
        var service = new RateLimiterService(rules);

        // Act
        bool token1Request = service.IsRequestAllowed("/test/endpoint", "client-token-1");
        bool token2Request = service.IsRequestAllowed("/test/endpoint", "client-token-2");

        // Assert
        Assert.True(token1Request);
        Assert.True(token2Request);
    }
}