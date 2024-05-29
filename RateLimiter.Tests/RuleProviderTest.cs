using NUnit.Framework;
using RateLimiter.Rules;
using RateLimiter.Services;
using RateLimiter.Tests.Helpers;
using System;

namespace RateLimiter.Tests;

[TestFixture]
public class RuleProviderTest
{
    #region RuleProvider Validation Tests

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void RuleProvider_AddRule_ThrowsArgumentException_WhenResourceIsInvalid(string resource)
    {
        var mockDateTimeWrapper = new MockDateTimeWrapper();
        var ruleProvider = new RuleProvider(mockDateTimeWrapper);
        var rule = new TimeSinceLastCallRule(TimeSpan.FromSeconds(1), mockDateTimeWrapper);

        Assert.Throws<ArgumentException>(() => ruleProvider.AddRule(resource, "US", rule));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void RuleProvider_AddRule_ThrowsArgumentException_WhenRegionIsInvalid(string region)
    {
        var mockDateTimeWrapper = new MockDateTimeWrapper();
        var ruleProvider = new RuleProvider(mockDateTimeWrapper);
        var rule = new TimeSinceLastCallRule(TimeSpan.FromSeconds(1), mockDateTimeWrapper);

        Assert.Throws<ArgumentException>(() => ruleProvider.AddRule("resource", region, rule));
    }

    [Test]
    public void RuleProvider_AddRule_ThrowsArgumentNullException_WhenRuleIsNull()
    {
        var mockDateTimeWrapper = new MockDateTimeWrapper();
        var ruleProvider = new RuleProvider(mockDateTimeWrapper);

        Assert.Throws<ArgumentNullException>(() => ruleProvider.AddRule("resource", "US", null!));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void RuleProvider_GetRulesForResource_ThrowsArgumentException_WhenResourceIsInvalid(string resource)
    {
        var mockDateTimeWrapper = new MockDateTimeWrapper();
        var ruleProvider = new RuleProvider(mockDateTimeWrapper);

        Assert.Throws<ArgumentException>(() => ruleProvider.GetRulesForResource(resource, "US"));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    public void RuleProvider_GetRulesForResource_ThrowsArgumentException_WhenRegionIsInvalid(string region)
    {
        var mockDateTimeWrapper = new MockDateTimeWrapper();
        var ruleProvider = new RuleProvider(mockDateTimeWrapper);

        Assert.Throws<ArgumentException>(() => ruleProvider.GetRulesForResource("resource", region));
    }

    #endregion
}
