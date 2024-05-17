using Moq;
using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.Rules.Info;
using System;

namespace RateLimiter.Tests.Rules;

[TestFixture]
public class ReqionBasedRuleTests
{
    private Mock<IRule> _innerRule = null!;
    private RegionBasedRule _USRegionBasedRule = null!;
    private const string US = "us";
    private const string EU = "eu";

    [SetUp]
    public void SetUp()
    {
        _innerRule = new Mock<IRule>();
        _USRegionBasedRule = new RegionBasedRule(US, _innerRule.Object);
    }

    [Test]
    public void Validate_SameRegionInnerRuleReturnsTrue_ReturnsTrue()
    {
        // Arrange
        const string region = US;
        var regionBasedInfo = new RegionBasedRuleInfo { Region = region  };
        _innerRule.Setup(x => x.Validate(It.IsAny<RuleRequestInfo>())).Returns(true);
        // Act
        
        var result = _USRegionBasedRule.Validate(regionBasedInfo);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Validate_DifferentRegionInnerRuleReturnsFalse_ReturnsTrue()
    {
        // Arrange
        const string region = EU;
        var regionBasedInfo = new RegionBasedRuleInfo { Region = region };
        _innerRule.Setup(x => x.Validate(It.IsAny<RuleRequestInfo>())).Returns(true);

        // Act
        var result = _USRegionBasedRule.Validate(regionBasedInfo);

        // Assert
        Assert.That(result, Is.True);
    }
}