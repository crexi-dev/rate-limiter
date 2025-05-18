using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Moq;
using RateLimiter.Common.Abstractions.Counters;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Models;
using RateLimiter.Core.Rules;
using RateLimiter.Core.Services.KeyBuilders;

namespace RateLimiter.UnitTests.Rules;

public class RegionBasedRuleTests
{
    private readonly Mock<IKeyBuilder> _keyBuilderMock;
    private readonly Mock<IRateLimitCounter> _counterMock;
    private readonly Mock<ILogger<RegionBasedRule>> _loggerMock;
    private readonly HttpContext _httpContext;

    public RegionBasedRuleTests()
    {
        _keyBuilderMock = new Mock<IKeyBuilder>();
        _counterMock = new Mock<IRateLimitCounter>();
        _loggerMock = new Mock<ILogger<RegionBasedRule>>();
        
        // Set up a default mock context
        _httpContext = new DefaultHttpContext();
        
        // Set up key builder to return a predictable key
        _keyBuilderMock
            .Setup(kg => kg.BuildKey(It.IsAny<HttpContext>(), It.IsAny<IRateLimitRule>(), It.IsAny<ClientIdentifier>()))
            .Returns("test-key");
    }

    [Fact]
    public async Task EvaluateAsync_WhenRegionMatches_ShouldApplyRateLimit()
    {
        // Arrange
        var rateLimit = new RateLimit { MaxRequests = 10, TimeWindowInSeconds = 60 };
        var rule = new RegionBasedRule(
            "TestRule",
            "US",
            rateLimit,
            _keyBuilderMock.Object,
            _counterMock.Object,
            _loggerMock.Object);
            
        var clientIdentifier = new ClientIdentifier
        {
            Id = "test-client",
            Region = "US"
        };
            
        // Set up mock counter to return counts below the limit
        _counterMock
            .Setup(c => c.GetCountAsync(It.IsAny<string>()))
            .ReturnsAsync(3); // Current count
            
        _counterMock
            .Setup(c => c.IncrementAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await rule.EvaluateAsync(_httpContext, clientIdentifier);

        // Assert
        Assert.True(result.IsAllowed);
        Assert.Equal("TestRule", result.Rule);
        Assert.Equal(10, result.Limit);
        Assert.Equal(4, result.Counter); // 3 + 1 for the current request
        
        // Verify the counter was incremented
        _counterMock.Verify(
            c => c.IncrementAsync(It.IsAny<string>(), 1, It.Is<TimeSpan>(ts => ts.TotalSeconds == 60)),
            Times.Once);
    }

    [Fact]
    public async Task EvaluateAsync_WhenRegionDoesNotMatch_ShouldSkipRateLimit()
    {
        // Arrange
        var rateLimit = new RateLimit { MaxRequests = 10, TimeWindowInSeconds = 60 };
        var rule = new RegionBasedRule(
            "TestRule",
            "US",
            rateLimit,
            _keyBuilderMock.Object,
            _counterMock.Object,
            _loggerMock.Object);
            
        var clientIdentifier = new ClientIdentifier
        {
            Id = "test-client",
            Region = "EU"
        };

        // Act
        var result = await rule.EvaluateAsync(_httpContext, clientIdentifier);

        // Assert
        Assert.True(result.IsAllowed);
        Assert.Equal("TestRule", result.Rule);
        Assert.Contains("doesn't match", result.Message ?? string.Empty);
        
        // Verify the counter was NOT used
        _counterMock.Verify(
            c => c.GetCountAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task EvaluateAsync_WithMinTimeBetweenRequests_ShouldEnforceMinimumTime()
    {
        // Arrange
        var rateLimit = new RateLimit { MaxRequests = 10, TimeWindowInSeconds = 60 };
        var minTimeBetweenRequests = 1000; // 1 second
        var rule = new RegionBasedRule(
            "TestRule",
            "EU",
            rateLimit,
            _keyBuilderMock.Object,
            _counterMock.Object,
            _loggerMock.Object,
            minTimeBetweenRequests);
            
        var clientIdentifier = new ClientIdentifier
        {
            Id = "test-client",
            Region = "EU"
        };
            
        // Set up last request time to be very recent (500ms ago)
        _counterMock
            .Setup(c => c.GetCountAsync(It.Is<string>(s => s.EndsWith(":lastReq"))))
            .ReturnsAsync(DateTimeOffset.UtcNow.AddMilliseconds(-500).ToUnixTimeMilliseconds());
            
        _counterMock
            .Setup(c => c.SetCountAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await rule.EvaluateAsync(_httpContext, clientIdentifier);

        // Assert
        Assert.False(result.IsAllowed);
        Assert.Equal("TestRule", result.Rule);
        Assert.Contains("Minimum time between requests", result.Message ?? string.Empty);
        
        // Verify the main counter was NOT checked
        _counterMock.Verify(
            c => c.GetCountAsync(It.Is<string>(s => !s.EndsWith(":lastReq"))),
            Times.Never);
    }
    
    [Fact]
    public async Task EvaluateAsync_WhenOverLimit_ShouldBlockRequest()
    {
        // Arrange
        var rateLimit = new RateLimit { MaxRequests = 10, TimeWindowInSeconds = 60 };
        var rule = new RegionBasedRule(
            "TestRule",
            "US",
            rateLimit,
            _keyBuilderMock.Object,
            _counterMock.Object,
            _loggerMock.Object);
            
        var clientIdentifier = new ClientIdentifier
        {
            Id = "test-client",
            Region = "US"
        };
            
        // Set up mock counter to return counts at limit
        _counterMock
            .Setup(c => c.GetCountAsync(It.Is<string>(s => !s.EndsWith(":lastReq"))))
            .ReturnsAsync(10); // At max requests
            
        // Act
        var result = await rule.EvaluateAsync(_httpContext, clientIdentifier);

        // Assert
        Assert.False(result.IsAllowed);
        Assert.Equal("TestRule", result.Rule);
        Assert.Equal(10, result.Counter);
        Assert.Equal(10, result.Limit);
        Assert.Contains("Rate limit exceeded for region", result.Message ?? string.Empty);
    }
}