using System;
using System.Collections.Generic;
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

public class LimitPerTimeRuleTests
{
    private const string ResourceName = "/test";

    private readonly LimitPerTimeRule _limitPerTimeRule;

    private readonly Mock<IRateLimitStore> _mockRateLimitStore;
    private readonly Mock<HttpContext> _mockHttpContext;

    public LimitPerTimeRuleTests()
    {
        _mockRateLimitStore = new Mock<IRateLimitStore>();
        _mockHttpContext = new Mock<HttpContext>();
        _limitPerTimeRule = new LimitPerTimeRule(_mockRateLimitStore.Object);
        
        _mockHttpContext.SetupProperty(x => x.Request.Path, new PathString(ResourceName));
    }

    [Fact]
    public async Task LimitPerTimeRule_ShouldAllowWhenEmpty()
    {
        _limitPerTimeRule.SetParameters(new LimitPerTimeRateLimitAttribute(10,10));
        _mockRateLimitStore
            .Setup(x => x.Get(ResourceName))
            .Returns(Task.FromResult(new List<RequestRateModel>() ));
        
        var result = await _limitPerTimeRule.IsRestrict(_mockHttpContext.Object);
            
        result.Should().Be(false);
    }
    
    [Fact]
    public async Task LimitPerTimeRule_ShouldAllow()
    {
        var data = new List<RequestRateModel>();
        data.Add( new RequestRateModel { RequestTicks = DateTime.Now.AddMinutes(-1).Ticks});
        _limitPerTimeRule.SetParameters(new LimitPerTimeRateLimitAttribute(10,10));
        _mockRateLimitStore
            .Setup(x => x.Get(ResourceName))
            .Returns(Task.FromResult(data));
        
        var result = await _limitPerTimeRule.IsRestrict(_mockHttpContext.Object);
            
        result.Should().Be(false);
    }
    
    [Fact]
    public async Task LimitPerTimeRule_ShouldRestrict()
    {
        var data = new List<RequestRateModel>();
        data.Add( new RequestRateModel { RequestTicks = DateTime.Now.AddSeconds(-5).Ticks});
        data.Add( new RequestRateModel { RequestTicks = DateTime.Now.AddSeconds(-2).Ticks});
        _limitPerTimeRule.SetParameters(new LimitPerTimeRateLimitAttribute(2,10));
        _mockRateLimitStore
            .Setup(x => x.Get(ResourceName))
            .Returns(Task.FromResult(data));
        
        var result = await _limitPerTimeRule.IsRestrict(_mockHttpContext.Object);
            
        result.Should().Be(true);
    }
    
    [Fact]
    public async Task LimitPerTimeRule_ShouldRestrictMoreThenNeeded()
    {
        var data = new List<RequestRateModel>();
        data.Add( new RequestRateModel { RequestTicks = DateTime.Now.AddSeconds(-5).Ticks});
        data.Add( new RequestRateModel { RequestTicks = DateTime.Now.AddSeconds(-2).Ticks});
        data.Add( new RequestRateModel { RequestTicks = DateTime.Now.AddSeconds(-1).Ticks});
        data.Add( new RequestRateModel { RequestTicks = DateTime.Now.AddSeconds(-3).Ticks});
        data.Add( new RequestRateModel { RequestTicks = DateTime.Now.AddSeconds(-4).Ticks});
        _limitPerTimeRule.SetParameters(new LimitPerTimeRateLimitAttribute(2,10));
        _mockRateLimitStore
            .Setup(x => x.Get(ResourceName))
            .Returns(Task.FromResult(data));
        
        var result = await _limitPerTimeRule.IsRestrict(_mockHttpContext.Object);
            
        result.Should().Be(true);
    }
    
    [Fact]
    public async Task LimitPerTimeRule_ShouldAllowByTimeDelta()
    {
        var data = new List<RequestRateModel>();
        data.Add( new RequestRateModel { RequestTicks = DateTime.Now.AddSeconds(-5).Ticks});
        data.Add( new RequestRateModel { RequestTicks = DateTime.Now.AddSeconds(-20).Ticks});
        data.Add( new RequestRateModel { RequestTicks = DateTime.Now.AddSeconds(-14).Ticks});
        data.Add( new RequestRateModel { RequestTicks = DateTime.Now.AddSeconds(-34).Ticks});
        _limitPerTimeRule.SetParameters(new LimitPerTimeRateLimitAttribute(2,10));
        _mockRateLimitStore
            .Setup(x => x.Get(ResourceName))
            .Returns(Task.FromResult(data));
        
        var result = await _limitPerTimeRule.IsRestrict(_mockHttpContext.Object);
            
        result.Should().Be(false);
    }
}