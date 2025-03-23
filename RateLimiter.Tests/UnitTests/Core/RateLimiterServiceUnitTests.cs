using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RateLimiter.Core;
using RateLimiter.Rules.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class RateLimiterServiceUnitTests //: IClassFixture<RateLimiterServiceUnitTests.TestSetup>
{ 
    private readonly Mock<ILogger<RateLimiterService>> _logger;
    private readonly string _token = "test-token";

    public RateLimiterServiceUnitTests()
    {
        _logger = new Mock<ILogger<RateLimiterService>>();
    }

    private DefaultHttpContext CreateHttpContext(string token, string path)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = $"Bearer {token}";
        context.Request.Path = path;
        return context;
    }

    private RateLimiterService CreateRateLimiterService(string endpoint, List<IRateLimiterRule> rules)
    {
        var mockConfigurationStorageProvider = new Mock<IConfigurationStorageProvider<IRateLimiterRule>>();
        mockConfigurationStorageProvider.Setup(repo => repo.LoadAsync(endpoint)).ReturnsAsync(rules);
        return new RateLimiterService(mockConfigurationStorageProvider.Object, _logger.Object);
    }

    [Fact]
    public async Task InvokeAsync_RequestAllowed_ReturnsTrue()
    {
        // Arrange
        var mockRuleA = new Mock<IRateLimiterRule>();

        mockRuleA.Setup(r => r.IsRequestAllowedAsync(It.IsAny<ClientRequestContext>())).ReturnsAsync(true);

        string endpoint = "/api/resourceA";
        var mockRulesConfig = new List<IRateLimiterRule> { mockRuleA.Object };

        var rateLimiter = CreateRateLimiterService(endpoint, mockRulesConfig);
        var context = CreateHttpContext(_token, endpoint);

        // Act
        var result = await rateLimiter.InvokeAsync(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task InvokeAsync_RequestNotAllowed_ReturnsFalse()
    {
        // Arrange
        var mockRuleA = new Mock<IRateLimiterRule>();
        mockRuleA.Setup(r => r.IsRequestAllowedAsync(It.IsAny<ClientRequestContext>())).ReturnsAsync(false);

        string endpoint = "/api/resourceA";
        var mockRulesConfig = new List<IRateLimiterRule> { mockRuleA.Object };

        var rateLimiter = CreateRateLimiterService(endpoint, mockRulesConfig);
        var context = CreateHttpContext(_token, endpoint);

        // Act
        var result = await rateLimiter.InvokeAsync(context);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task InvokeAsync_RequestAllowed_MultipleRules_ReturnsTrue()
    {
        // Arrange

        var mockRuleA = new Mock<IRateLimiterRule>();
        mockRuleA.Setup(r => r.IsRequestAllowedAsync(It.IsAny<ClientRequestContext>())).ReturnsAsync(true);

        var mockRuleB = new Mock<IRateLimiterRule>();
        mockRuleB.Setup(r => r.IsRequestAllowedAsync(It.IsAny<ClientRequestContext>())).ReturnsAsync(true);

        var mockRuleC = new Mock<IRateLimiterRule>();
        mockRuleC.Setup(r => r.IsRequestAllowedAsync(It.IsAny<ClientRequestContext>())).ReturnsAsync(true);

        string endpoint = "/api/resourceA";

        var mockRulesConfig = new List<IRateLimiterRule> { mockRuleA.Object, mockRuleB.Object, mockRuleC.Object };

        var rateLimiter = CreateRateLimiterService(endpoint, mockRulesConfig);
        var context = CreateHttpContext(_token, endpoint);

        // Act
        var result = await rateLimiter.InvokeAsync(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task InvokeAsync_RequestNotAllowed_MultipleRules_ReturnsFalse()
    {
        // Arrange
        var mockRuleA = new Mock<IRateLimiterRule>();
        mockRuleA.Setup(r => r.IsRequestAllowedAsync(It.IsAny<ClientRequestContext>())).ReturnsAsync(true);

        var mockRuleB = new Mock<IRateLimiterRule>();
        mockRuleB.Setup(r => r.IsRequestAllowedAsync(It.IsAny<ClientRequestContext>())).ReturnsAsync(false);    // block request

        var mockRuleC = new Mock<IRateLimiterRule>();
        mockRuleC.Setup(r => r.IsRequestAllowedAsync(It.IsAny<ClientRequestContext>())).ReturnsAsync(true);

        string endpoint = "/api/resourceA";

        var mockRulesConfig = new List<IRateLimiterRule> { mockRuleA.Object, mockRuleB.Object, mockRuleC.Object };

        var rateLimiter = CreateRateLimiterService(endpoint, mockRulesConfig);
        var context = CreateHttpContext(_token, endpoint);

        // Act
        var result = await rateLimiter.InvokeAsync(context);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task InvokeAsync_EndpointWithoutConfig_RequestAllowed_ReturnsTrue()
    {
        // Arrange
        string endpoint = "/api/resourceX";

        var mockRulesConfig = new List<IRateLimiterRule>{};     // no rules configured for this endpoint


        var rateLimiter = CreateRateLimiterService(endpoint, mockRulesConfig);
        var context = CreateHttpContext(_token, endpoint);

        // Act
        var result = await rateLimiter.InvokeAsync(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task InvokeAsync_EndpointNotFound_RequestAllowed_ReturnsTrue()
    {
        // Arrange
        string endpoint = "/api/resourceX";

        var rateLimiter = CreateRateLimiterService("", new List<IRateLimiterRule> { });    // no endpoint and rules configured
        var context = CreateHttpContext(_token, endpoint);

        // Act
        var result = await rateLimiter.InvokeAsync(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task InvokeAsync_EmptyEndpoint_ReturnsFalse()
    {
        // Arrange
        string endpoint = string.Empty;
        var mockRulesConfig = new List<IRateLimiterRule>();

        var rateLimiter = CreateRateLimiterService(endpoint, mockRulesConfig);
        var context = CreateHttpContext(_token, endpoint);

        // Act
        var result = await rateLimiter.InvokeAsync(context);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task InvokeAsync_NoClientTokenProvided_Returnsfalse()
    {
        // Arrange
        string endpoint = "/api/resourceA";
        var mockRulesConfig = new List<IRateLimiterRule>();

        var rateLimiter = CreateRateLimiterService(endpoint, mockRulesConfig);
        var context = new DefaultHttpContext();
        context.Request.Path = endpoint;

        // Act
        var result = await rateLimiter.InvokeAsync(context);

        // Assert
        Assert.False(result);
    }
}
