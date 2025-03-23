using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RateLimiter.ConfigurationStorageProvider;
using RateLimiter.Core;
using RateLimiter.Rules.Interfaces;
using RateLimiter.Rules.Implementations.RequestPerPeriodRule;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Xunit;


namespace RateLimiter.IntegrationTests.Rules.RequestPerPeriodRule;

public class RequestPerPeriodRuleTests
{
    
    private readonly int NUMBER_OF_REQUESTS_ENDPOINT_A = 2;
    private readonly int PERIOD_ENDPOINT_A = 10;               // seconds
    private readonly int NUMBER_OF_REQUESTS_ENDPOINT_B = 2;
    private readonly int PERIOD_ENDPOINT_B = 60;               // seconds
    private readonly Mock<ILogger<RateLimiterService>> _logger;
    private readonly InMemoryConfigurationStorage _memoryStorage;
    private readonly ConfigurationProvider _configurationProvider;

    public RequestPerPeriodRuleTests()
    {
        
        _logger = new Mock<ILogger<RateLimiterService>>();

        _memoryStorage = new InMemoryConfigurationStorage();
        _configurationProvider = new ConfigurationProvider(_memoryStorage);

        SetupConfig();

    }

    private async void SetupConfig()
    {
        var ruleRepository = new InMemoryRequestPerPeriodRuleRepository();

        // Save configuration
        await _configurationProvider.SaveConfigAsync("/api/resourceA", new List<IRateLimiterRule> {
            new RequestsPerPeriodRule(NUMBER_OF_REQUESTS_ENDPOINT_A, TimeSpan.FromSeconds(PERIOD_ENDPOINT_A), ruleRepository)
        });

        await _configurationProvider.SaveConfigAsync("/api/resourceB", new List<IRateLimiterRule> {
            new RequestsPerPeriodRule(NUMBER_OF_REQUESTS_ENDPOINT_B, TimeSpan.FromSeconds(PERIOD_ENDPOINT_B), ruleRepository)
        });
    }

    private DefaultHttpContext CreateHttpContext(string token, string path)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = $"Bearer {token}";
        context.Request.Path = path;
        return context;
    }

    private RateLimiterService CreateRateLimiterService()
    {
        return new RateLimiterService(_memoryStorage, _logger.Object);
    }

    [Fact]
    public async Task InvokeAsync_EndPointA_RequestAllowed_ReturnsTrue()
    {
        // Arrange
        var context = CreateHttpContext("test-token", "/api/resourceA");

        await Task.Delay((PERIOD_ENDPOINT_A * 1000) + 1); // Simulate async operation

        var RateLimiterService = CreateRateLimiterService();
        
        // Act
        var result = await RateLimiterService.InvokeAsync(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task InvokeAsync_EndPointA_RequestNotAllowed_ReturnsFalse()
    {
        // Arrange
        var context = CreateHttpContext("test-token", "/api/resourceA");

        var RateLimiterService = CreateRateLimiterService();

        // Simulate a request to set the last call time
        await RateLimiterService.InvokeAsync(context);

        await Task.Delay(1); // Simulate async operation

        // Act
        var result = await RateLimiterService.InvokeAsync(context);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task InvokeAsync_EndPointB_RequestAllowed_ReturnsTrue()
    {
        // Arrange
        var context = CreateHttpContext("test-token", "/api/resourceB");
        var RateLimiterService = CreateRateLimiterService();

        // Simulate a request to set the last call time
        for (int i = 1; i < NUMBER_OF_REQUESTS_ENDPOINT_B -1; i++)
        {
            await RateLimiterService.InvokeAsync(context);
        }
        // Act
        var result = await RateLimiterService.InvokeAsync(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task InvokeAsync_EndPointB_RequestNotAllowed_ReturnsFalse()
    {
        // Arrange
        var context = CreateHttpContext("test-token", "/api/resourceB");
        var RateLimiterService = CreateRateLimiterService();

        // Simulate a request to set the last call time
        for (int i = 1; i < NUMBER_OF_REQUESTS_ENDPOINT_B + 10; i++)
        {
            await RateLimiterService.InvokeAsync(context);
        }
        
        // Act
        var result = await RateLimiterService.InvokeAsync(context);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task InvokeAsync_NoRulesDefined_ReturnsTrue()
    {
        // Arrange
        var context = CreateHttpContext("test-token", "/api/unknown");
        var RateLimiterService = CreateRateLimiterService();

        // Act
        var result = await RateLimiterService.InvokeAsync(context);

        // Assert
        Assert.True(result);
    }

}
