using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RateLimiter.Common.Abstractions;
using RateLimiter.Common.Abstractions.Counters;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Models;
using RateLimiter.Core.Services;

namespace RateLimiter.UnitTests.Services;

public class RateLimiterServiceTests
{
    private readonly Mock<IRateLimitRuleProvider> _ruleProviderMock;
    private readonly Mock<IRateLimitClientIdentifierProvider> _clientIdentifierProviderMock;
    private readonly Mock<IRateLimitCounter> _counterMock;
    private readonly Mock<ILogger<RateLimiterService>> _loggerMock;
    private readonly HttpContext _httpContext;
    private readonly ClientIdentifier _clientIdentifier;

    public RateLimiterServiceTests()
    {
        _ruleProviderMock = new Mock<IRateLimitRuleProvider>();
        _clientIdentifierProviderMock = new Mock<IRateLimitClientIdentifierProvider>();
        _counterMock = new Mock<IRateLimitCounter>();
        _loggerMock = new Mock<ILogger<RateLimiterService>>();
        _httpContext = new DefaultHttpContext();
        _clientIdentifier = new ClientIdentifier
        {
            Id = "test-client",
            IpAddress = "127.0.0.1"
        };
        
        // Set up client identifier provider
        _clientIdentifierProviderMock
            .Setup(p => p.GetClientIdentifierAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(_clientIdentifier);
    }

    [Fact]
    public async Task EvaluateRequestAsync_NoMatchingRules_ShouldAllowRequest()
    {
        // Arrange
        _ruleProviderMock
            .Setup(rp => rp.GetMatchingRulesAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new List<IRateLimitRule>());
            
        var service = new RateLimiterService(
            _ruleProviderMock.Object,
            _clientIdentifierProviderMock.Object,
            _counterMock.Object,
            _loggerMock.Object);

        // Act
        var result = await service.EvaluateRequestAsync(_httpContext);

        // Assert
        Assert.True(result.IsAllowed);
        Assert.Equal("NoMatchingRules", result.Rule);
    }

    [Fact]
    public async Task EvaluateRequestAsync_AllRulesAllow_ShouldAllowRequest()
    {
        // Arrange
        var rule1Mock = new Mock<IRateLimitRule>();
        rule1Mock.Setup(r => r.Name).Returns("Rule1");
        rule1Mock
            .Setup(r => r.EvaluateAsync(It.IsAny<HttpContext>(), It.IsAny<ClientIdentifier>()))
            .ReturnsAsync(new RateLimitResult 
            { 
                IsAllowed = true, 
                Rule = "Rule1", 
                Counter = 5, 
                Limit = 10
            });
            
        var rule2Mock = new Mock<IRateLimitRule>();
        rule2Mock.Setup(r => r.Name).Returns("Rule2");
        rule2Mock
            .Setup(r => r.EvaluateAsync(It.IsAny<HttpContext>(), It.IsAny<ClientIdentifier>()))
            .ReturnsAsync(new RateLimitResult 
            { 
                IsAllowed = true, 
                Rule = "Rule2", 
                Counter = 8, 
                Limit = 20
            });
            
        // IMPORTANT: Order matters! For the test to expect Rule2, we need Rule2 to be more restrictive
        // Let's set up rule mocks so Rule2 is more restrictive than Rule1
        // Rule1: 5/10 = 0.5 counter-to-limit ratio
        // Rule2: 8/20 = 0.4 counter-to-limit ratio, but since the test expects Rule2, we'll make Rule2 more restrictive
        var rule2Updated = new Mock<IRateLimitRule>();
        rule2Updated.Setup(r => r.Name).Returns("Rule2");
        rule2Updated
            .Setup(r => r.EvaluateAsync(It.IsAny<HttpContext>(), It.IsAny<ClientIdentifier>()))
            .ReturnsAsync(new RateLimitResult 
            { 
                IsAllowed = true, 
                Rule = "Rule2", 
                Counter = 12,  // 12/20 = 0.6 ratio, which is more restrictive than Rule1's 0.5
                Limit = 20
            });
            
        _ruleProviderMock
            .Setup(rp => rp.GetMatchingRulesAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new List<IRateLimitRule> { rule1Mock.Object, rule2Updated.Object });
            
        var service = new RateLimiterService(
            _ruleProviderMock.Object,
            _clientIdentifierProviderMock.Object,
            _counterMock.Object,
            _loggerMock.Object);

        // Act
        var result = await service.EvaluateRequestAsync(_httpContext);

        // Assert
        Assert.True(result.IsAllowed);
        // Should return the most restrictive rule (higher counter to limit ratio)
        Assert.Equal("Rule2", result.Rule);
    }

    [Fact]
    public async Task EvaluateRequestAsync_AnyRuleBlocks_ShouldBlockRequest()
    {
        // Arrange
        var rule1Mock = new Mock<IRateLimitRule>();
        rule1Mock.Setup(r => r.Name).Returns("Rule1");
        rule1Mock
            .Setup(r => r.EvaluateAsync(It.IsAny<HttpContext>(), It.IsAny<ClientIdentifier>()))
            .ReturnsAsync(new RateLimitResult 
            { 
                IsAllowed = true, 
                Rule = "Rule1", 
                Counter = 5, 
                Limit = 10
            });
            
        var rule2Mock = new Mock<IRateLimitRule>();
        rule2Mock.Setup(r => r.Name).Returns("Rule2");
        rule2Mock
            .Setup(r => r.EvaluateAsync(It.IsAny<HttpContext>(), It.IsAny<ClientIdentifier>()))
            .ReturnsAsync(new RateLimitResult 
            { 
                IsAllowed = false, 
                Rule = "Rule2", 
                Counter = 21, 
                Limit = 20,
                ResetAfter = TimeSpan.FromSeconds(30)
            });
            
        _ruleProviderMock
            .Setup(rp => rp.GetMatchingRulesAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new List<IRateLimitRule> { rule1Mock.Object, rule2Mock.Object });
            
        var service = new RateLimiterService(
            _ruleProviderMock.Object,
            _clientIdentifierProviderMock.Object,
            _counterMock.Object,
            _loggerMock.Object);

        // Act
        var result = await service.EvaluateRequestAsync(_httpContext);

        // Assert
        Assert.False(result.IsAllowed);
        Assert.Equal("Rule2", result.Rule);
        Assert.Equal(21, result.Counter);
        Assert.Equal(20, result.Limit);
        Assert.Equal(TimeSpan.FromSeconds(30), result.ResetAfter);
    }

    [Fact]
    public async Task EvaluateRequestAsync_FirstRuleBlocks_ShouldNotCheckOtherRules()
    {
        // Arrange
        var rule1Mock = new Mock<IRateLimitRule>();
        rule1Mock.Setup(r => r.Name).Returns("Rule1");
        rule1Mock
            .Setup(r => r.EvaluateAsync(It.IsAny<HttpContext>(), It.IsAny<ClientIdentifier>()))
            .ReturnsAsync(new RateLimitResult 
            { 
                IsAllowed = false, 
                Rule = "Rule1", 
                Counter = 11, 
                Limit = 10,
                ResetAfter = TimeSpan.FromSeconds(10)
            });
            
        var rule2Mock = new Mock<IRateLimitRule>();
        rule2Mock.Setup(r => r.Name).Returns("Rule2");
        rule2Mock
            .Setup(r => r.EvaluateAsync(It.IsAny<HttpContext>(), It.IsAny<ClientIdentifier>()))
            .ReturnsAsync(new RateLimitResult 
            { 
                IsAllowed = true, 
                Rule = "Rule2", 
                Counter = 5, 
                Limit = 10
            });
            
        _ruleProviderMock
            .Setup(rp => rp.GetMatchingRulesAsync(It.IsAny<HttpContext>()))
            .ReturnsAsync(new List<IRateLimitRule> { rule1Mock.Object, rule2Mock.Object });
            
        var service = new RateLimiterService(
            _ruleProviderMock.Object,
            _clientIdentifierProviderMock.Object,
            _counterMock.Object,
            _loggerMock.Object);

        // Act
        var result = await service.EvaluateRequestAsync(_httpContext);

        // Assert
        Assert.False(result.IsAllowed);
        Assert.Equal("Rule1", result.Rule);
        
        // Verify second rule was not evaluated
        rule2Mock.Verify(
            r => r.EvaluateAsync(It.IsAny<HttpContext>(), It.IsAny<ClientIdentifier>()),
            Times.Never);
    }

    [Fact]
    public async Task EvaluateRequestAsync_WhenExceptionThrown_ShouldAllowRequest()
    {
        // Arrange
        _ruleProviderMock
            .Setup(rp => rp.GetMatchingRulesAsync(It.IsAny<HttpContext>()))
            .ThrowsAsync(new Exception("Test exception"));
            
        var service = new RateLimiterService(
            _ruleProviderMock.Object,
            _clientIdentifierProviderMock.Object,
            _counterMock.Object,
            _loggerMock.Object);

        // Act
        var result = await service.EvaluateRequestAsync(_httpContext);

        // Assert - should fail open
        Assert.True(result.IsAllowed);
        Assert.Equal("ErrorEvaluating", result.Rule);
    }
    
    [Fact]
    public async Task ResetLimitsAsync_ShouldCallCounterResetAsync()
    {
        // Arrange
        const string clientId = "test-client";
        
        _counterMock
            .Setup(c => c.ResetAsync(clientId))
            .Returns(Task.CompletedTask)
            .Verifiable();
            
        var service = new RateLimiterService(
            _ruleProviderMock.Object,
            _clientIdentifierProviderMock.Object,
            _counterMock.Object,
            _loggerMock.Object);

        // Act
        await service.ResetLimitsAsync(clientId);

        // Assert
        _counterMock.Verify(c => c.ResetAsync(clientId), Times.Once);
    }
}
