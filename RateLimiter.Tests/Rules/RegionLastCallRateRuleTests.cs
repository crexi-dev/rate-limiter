using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using RateLimiter.Attributes;
using RateLimiter.Rules;
using RateLimiter.Rules.Interfaces;
using Xunit;

namespace RateLimiter.Tests.Rules;

public class RegionLastCallRateRuleTests
{
    private readonly RegionLastCallRateRule _regionLastCallRateRule;

    private readonly Mock<IRule<LastCallRateLimitAttribute>> _mockLastCallRule;
    private readonly Mock<IRule<RegionLimitAttribute>> _mockRegionLimitRule;
    
    private readonly Mock<HttpContext> _mockHttpContext;

    public RegionLastCallRateRuleTests()
    {
        _mockHttpContext = new Mock<HttpContext>();
        _mockLastCallRule = new Mock<IRule<LastCallRateLimitAttribute>>();
        _mockRegionLimitRule = new Mock<IRule<RegionLimitAttribute>>();
        _regionLastCallRateRule = new RegionLastCallRateRule(_mockLastCallRule.Object, _mockRegionLimitRule.Object);
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public async Task RegionLastCallRateRule(bool expected, bool lastCall, bool region)
    {
        _regionLastCallRateRule.SetParameters(new RegionLastCallRateLimitAttribute(10,"RU"));
        _mockLastCallRule
            .Setup(x => x.IsRestrict(It.IsAny<HttpContext>()))
            .Returns(Task.FromResult(lastCall));
        
        _mockRegionLimitRule
            .Setup(x => x.IsRestrict(It.IsAny<HttpContext>()))
            .Returns(Task.FromResult(region));
        
        var result = await _regionLastCallRateRule.IsRestrict(_mockHttpContext.Object);
            
        result.Should().Be(expected);
    }
}