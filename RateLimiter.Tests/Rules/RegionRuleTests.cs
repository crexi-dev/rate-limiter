using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using RateLimiter.Attributes;
using RateLimiter.Contexts.Interfaces;
using RateLimiter.Rules;
using Xunit;

namespace RateLimiter.Tests.Rules;

public class RegionRuleTests
{
    private const string ResourceName = "/test";

    
    private readonly RegionRule _regionRule;

    private readonly Mock<IUserContext> _mockUserContext;
    private readonly Mock<HttpContext> _mockHttpContext;

    public RegionRuleTests()
    {
        _mockUserContext = new Mock<IUserContext>();
        _mockHttpContext = new Mock<HttpContext>();
        _regionRule = new RegionRule(_mockUserContext.Object);
        
        _mockHttpContext.SetupProperty(x => x.Request.Path, new PathString(ResourceName));

    }

    [Fact]
    public async Task RegionRule_ShouldAllowWhenEmpty()
    {
        _regionRule.SetParameters(new RegionLimitAttribute());
        _mockUserContext.SetupProperty(x => x.Region, "US");
        
        var result = await _regionRule.IsRestrict(_mockHttpContext.Object);
            
        result.Should().Be(false);
    }
    
    [Fact]
    public async Task RegionRule_ShouldAllow()
    {
        _regionRule.SetParameters(new RegionLimitAttribute("RU"));
        _mockUserContext.SetupProperty(x => x.Region, "US");
        
        var result = await _regionRule.IsRestrict(_mockHttpContext.Object);
            
        result.Should().Be(false);
    }
    
    [Fact]
    public async Task RegionRule_ShouldRestrict()
    {
        _regionRule.SetParameters(new RegionLimitAttribute("US"));
        _mockUserContext.SetupProperty(x => x.Region, "US");
        
        var result = await _regionRule.IsRestrict(_mockHttpContext.Object);
            
        result.Should().Be(true);
    }
    
    [Fact]
    public async Task RegionRule_ShouldRestrictWhenManyRegionsRestricted()
    {
        _regionRule.SetParameters(new RegionLimitAttribute("US", "RU"));
        _mockUserContext.SetupProperty(x => x.Region, "US");
        
        var result = await _regionRule.IsRestrict(_mockHttpContext.Object);
            
        result.Should().Be(true);
    }
}