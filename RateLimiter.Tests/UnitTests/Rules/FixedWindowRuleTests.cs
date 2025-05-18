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

public class FixedWindowRuleTests
{
    private readonly Mock<IKeyBuilder> _keyBuilderMock;
    private readonly Mock<IRateLimitCounter> _counterMock;
    private readonly Mock<ILogger<FixedWindowRule>> _loggerMock;
    private readonly HttpContext _httpContext;
    private readonly ClientIdentifier _clientIdentifier;

    public FixedWindowRuleTests()
    {
        _keyBuilderMock = new Mock<IKeyBuilder>();
        _counterMock = new Mock<IRateLimitCounter>();
        _loggerMock = new Mock<ILogger<FixedWindowRule>>();
        
        // Set up a default mock context
        _httpContext = new DefaultHttpContext();
        
        // Set up a default client identifier
        _clientIdentifier = new ClientIdentifier
        {
            Id = "test-client",
            IpAddress = "127.0.0.1"
        };
        
        // Set up key builder to return a predictable key
        _keyBuilderMock
            .Setup(kg => kg.BuildKey(It.IsAny<HttpContext>(), It.IsAny<IRateLimitRule>(), It.IsAny<ClientIdentifier>()))
            .Returns("test-key");
    }

    [Fact]
    public async Task EvaluateAsync_WhenBelowLimit_ShouldAllowRequest()
    {
        // Arrange
        var rateLimit = new RateLimit { MaxRequests = 10, TimeWindowInSeconds = 60 };
        var rule = new FixedWindowRule(
            "TestRule",
            rateLimit,
            _keyBuilderMock.Object,
            _counterMock.Object,
            _loggerMock.Object);
            
        // Set up mock counter to return counts below the limit
        _counterMock
            .Setup(c => c.GetCountAsync(It.IsAny<string>()))
            .ReturnsAsync(3); // Current count
            
        _counterMock
            .Setup(c => c.IncrementAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await rule.EvaluateAsync(_httpContext, _clientIdentifier);

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
    public async Task EvaluateAsync_WhenAtLimit_ShouldBlockRequest()
    {
        // Arrange
        var rateLimit = new RateLimit { MaxRequests = 10, TimeWindowInSeconds = 60 };
        var rule = new FixedWindowRule(
            "TestRule",
            rateLimit,
            _keyBuilderMock.Object,
            _counterMock.Object,
            _loggerMock.Object);
            
        // Set up mock counter to return counts at the limit
        _counterMock
            .Setup(c => c.GetCountAsync(It.IsAny<string>()))
            .ReturnsAsync(10); // Already at max requests
            
        // Act
        var result = await rule.EvaluateAsync(_httpContext, _clientIdentifier);

        // Assert
        Assert.False(result.IsAllowed);
        Assert.Equal("TestRule", result.Rule);
        Assert.Equal(10, result.Limit);
        Assert.Equal(10, result.Counter);
        
        // Verify the counter was NOT incremented
        _counterMock.Verify(
            c => c.IncrementAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<TimeSpan>()),
            Times.Never);
    }
    
    [Fact]
    public async Task EvaluateAsync_WhenExceptionThrown_ShouldAllowRequest()
    {
        // Arrange
        var rateLimit = new RateLimit { MaxRequests = 10, TimeWindowInSeconds = 60 };
        var rule = new FixedWindowRule(
            "TestRule",
            rateLimit,
            _keyBuilderMock.Object,
            _counterMock.Object,
            _loggerMock.Object);
            
        // Set up counter to throw an exception
        _counterMock
            .Setup(c => c.GetCountAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await rule.EvaluateAsync(_httpContext, _clientIdentifier);

        // Assert - should fail open
        Assert.True(result.IsAllowed);
        Assert.Equal("TestRule", result.Rule);
    }
    
    [Fact]
    public void IsMatch_WithNoMatcher_ShouldMatchAllRequests()
    {
        // Arrange
        var rateLimit = new RateLimit { MaxRequests = 10, TimeWindowInSeconds = 60 };
        var rule = new FixedWindowRule(
            "TestRule",
            rateLimit,
            _keyBuilderMock.Object,
            _counterMock.Object,
            _loggerMock.Object);

        // Act
        var result = rule.IsMatch(_httpContext);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void IsMatch_WithMatcherReturningTrue_ShouldMatchRequest()
    {
        // Arrange
        var rateLimit = new RateLimit { MaxRequests = 10, TimeWindowInSeconds = 60 };
        var rule = new FixedWindowRule(
            "TestRule",
            rateLimit,
            _keyBuilderMock.Object,
            _counterMock.Object,
            _loggerMock.Object,
            _ => true);

        // Act
        var result = rule.IsMatch(_httpContext);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void IsMatch_WithMatcherReturningFalse_ShouldNotMatchRequest()
    {
        // Arrange
        var rateLimit = new RateLimit { MaxRequests = 10, TimeWindowInSeconds = 60 };
        var rule = new FixedWindowRule(
            "TestRule",
            rateLimit,
            _keyBuilderMock.Object,
            _counterMock.Object,
            _loggerMock.Object,
            _ => false);

        // Act
        var result = rule.IsMatch(_httpContext);

        // Assert
        Assert.False(result);
    }
}
