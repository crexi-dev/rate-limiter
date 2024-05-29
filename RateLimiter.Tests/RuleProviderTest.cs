using Moq;
using NUnit.Framework;
using RateLimiterRules.Rules;
using RateLimiter.Services;
using System;
using RateLimiter.Interfaces;

namespace RateLimiter.Tests;

[TestFixture]
public class RuleProviderTest
{
    private Mock<IDateTimeWrapper> _dateTimeWrapperMock;

    [SetUp]
    public void SetUp()
    {
        _dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
    }

    #region RuleProvider Validation Tests

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void RuleProvider_AddRule_ThrowsArgumentException_WhenResourceIsInvalid(string resource)
    {
        var ruleProvider = new RuleProvider(_dateTimeWrapperMock.Object);
        var rule = new TimeSinceLastCallRule(TimeSpan.FromSeconds(1), _dateTimeWrapperMock.Object);

        Assert.Throws<ArgumentException>(() => ruleProvider.AddRule(resource, "US", rule));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void RuleProvider_AddRule_ThrowsArgumentException_WhenRegionIsInvalid(string region)
    {
        var ruleProvider = new RuleProvider(_dateTimeWrapperMock.Object);
        var rule = new TimeSinceLastCallRule(TimeSpan.FromSeconds(1), _dateTimeWrapperMock.Object);

        Assert.Throws<ArgumentException>(() => ruleProvider.AddRule("resource", region, rule));
    }

    [Test]
    public void RuleProvider_AddRule_ThrowsArgumentNullException_WhenRuleIsNull()
    {
        var ruleProvider = new RuleProvider(_dateTimeWrapperMock.Object);

        Assert.Throws<ArgumentNullException>(() => ruleProvider.AddRule("resource", "US", null!));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void RuleProvider_GetRulesForResource_ThrowsArgumentException_WhenResourceIsInvalid(string resource)
    {
        var ruleProvider = new RuleProvider(_dateTimeWrapperMock.Object);

        Assert.Throws<ArgumentException>(() => ruleProvider.GetRulesForResource(resource, "US"));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void RuleProvider_GetRulesForResource_ThrowsArgumentException_WhenRegionIsInvalid(string region)
    {
        var ruleProvider = new RuleProvider(_dateTimeWrapperMock.Object);

        Assert.Throws<ArgumentException>(() => ruleProvider.GetRulesForResource("resource", region));
    }

    #endregion
}