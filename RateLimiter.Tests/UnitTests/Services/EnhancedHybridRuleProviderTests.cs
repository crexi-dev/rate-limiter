using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Models;
using RateLimiter.Core.Configuration;
using RateLimiter.Core.Services;

namespace RateLimiter.UnitTests.Services;

public class EnhancedHybridRuleProviderTests
{
    private readonly Mock<ILogger<EnhancedHybridRuleProvider>> _loggerMock;
    private readonly EnhancedRateLimitConfiguration _config;
    private readonly HttpContext _httpContext;

    public EnhancedHybridRuleProviderTests()
    {
        _loggerMock = new Mock<ILogger<EnhancedHybridRuleProvider>>();
        _httpContext = new DefaultHttpContext();
        _httpContext.Request.Path = "/api/demo";
        
        _config = new EnhancedRateLimitConfiguration
        {
            EnableConfigurationRules = true,
            EnableAttributeRules = true,
            ConflictResolutionStrategy = ConflictResolutionStrategy.ConfigurationWins,
            LogConflicts = true
        };
    }

    [Fact]
    public async Task GetAllRulesAsync_WithNoProviders_ShouldReturnEmptyCollection()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<EnhancedRateLimitConfiguration>>();
        optionsMock.Setup(o => o.Value).Returns(_config);

        var provider = new EnhancedHybridRuleProvider(
            optionsMock.Object,
            _loggerMock.Object,
            null, // No config provider
            null  // No attribute provider
        );

        // Act
        var result = await provider.GetAllRulesAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMatchingRulesAsync_WithNoProviders_ShouldReturnEmptyCollection()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<EnhancedRateLimitConfiguration>>();
        optionsMock.Setup(o => o.Value).Returns(_config);

        var provider = new EnhancedHybridRuleProvider(
            optionsMock.Object,
            _loggerMock.Object,
            null, // No config provider
            null  // No attribute provider
        );

        // Act
        var result = await provider.GetMatchingRulesAsync(_httpContext);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMatchingRulesAsync_WithConfigurationDisabled_ShouldReturnEmptyCollection()
    {
        // Arrange
        _config.EnableConfigurationRules = false;
        _config.EnableAttributeRules = false;

        var optionsMock = new Mock<IOptions<EnhancedRateLimitConfiguration>>();
        optionsMock.Setup(o => o.Value).Returns(_config);

        var provider = new EnhancedHybridRuleProvider(
            optionsMock.Object,
            _loggerMock.Object,
            null,
            null
        );

        // Act
        var result = await provider.GetMatchingRulesAsync(_httpContext);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldNotThrow()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<EnhancedRateLimitConfiguration>>();
        optionsMock.Setup(o => o.Value).Returns(_config);

        // Act & Assert
        var exception = Record.Exception(() => new EnhancedHybridRuleProvider(
            optionsMock.Object,
            _loggerMock.Object,
            null,
            null
        ));

        Assert.Null(exception);
    }

    [Fact]
    public void Constructor_WithNullConfig_ShouldThrow()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EnhancedHybridRuleProvider(
            null!,
            _loggerMock.Object,
            null,
            null
        ));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrow()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<EnhancedRateLimitConfiguration>>();
        optionsMock.Setup(o => o.Value).Returns(_config);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EnhancedHybridRuleProvider(
            optionsMock.Object,
            null!,
            null,
            null
        ));
    }

    private Mock<IRateLimitRule> CreateMockRule(string name, int maxRequests, int timeWindow = 60)
    {
        var rule = new Mock<IRateLimitRule>();
        rule.Setup(r => r.Name).Returns(name);
        rule.Setup(r => r.GetLimit(It.IsAny<HttpContext>()))
            .Returns(new RateLimit { MaxRequests = maxRequests, TimeWindowInSeconds = timeWindow });
        return rule;
    }
}
