using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using RateLimiter.Attributes;
using RateLimiter.Rules;
using RateLimiter.Stores.Interfaces;
using RateLimiter.Stores.Models;
using Xunit;

namespace RateLimiter.Tests.Rules;

public class LastCallRuleTests
{
    private const string ResourceName = "/test";
    
    private readonly LastCallRule _lastCallRule;

    private readonly Mock<IRateLimitStore> _mockRateLimitStore;
    private readonly Mock<HttpContext> _mockHttpContext;

    public LastCallRuleTests()
    {
        _mockRateLimitStore = new Mock<IRateLimitStore>();
        _mockHttpContext = new Mock<HttpContext>();
        _lastCallRule = new LastCallRule(_mockRateLimitStore.Object);
        
        _mockHttpContext.SetupProperty(x => x.Request.Path, new PathString(ResourceName));

    }

    [Fact]
    public async Task LastCallRule_ShouldAllowWhenEmpty()
    {
        RequestRateModel? data = null;
        _lastCallRule.SetParameters(new LastCallRateLimitAttribute(10));
        _mockRateLimitStore
            .Setup(x => x.GetLast(ResourceName))
            .Returns(Task.FromResult(data));
        
        var result = await _lastCallRule.IsRestrict(_mockHttpContext.Object);
            
        result.Should().Be(false);
    }
    
    [Fact]
    public async Task LastCallRule_ShouldAllow()
    {
        var data = new RequestRateModel { RequestTicks = DateTime.Now.AddMinutes(-1).Ticks};
        _lastCallRule.SetParameters(new LastCallRateLimitAttribute(10));
        _mockRateLimitStore
            .Setup(x => x.GetLast(ResourceName))
            .Returns(Task.FromResult(data)!);
        
        var result = await _lastCallRule.IsRestrict(_mockHttpContext.Object);
            
        result.Should().Be(false);
    }
    
    [Fact]
    public async Task LastCallRule_ShouldRestrict()
    {
        var data = new RequestRateModel { RequestTicks = DateTime.Now.AddSeconds(-5).Ticks};
        _lastCallRule.SetParameters(new LastCallRateLimitAttribute(10));
        _mockRateLimitStore
            .Setup(x => x.GetLast(ResourceName))
            .Returns(Task.FromResult(data)!);
        
        var result = await _lastCallRule.IsRestrict(_mockHttpContext.Object);
            
        result.Should().Be(true);
    }
}