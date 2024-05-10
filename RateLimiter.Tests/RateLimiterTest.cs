using NUnit.Framework;
using RateLimiter.Classes;
using RateLimiter.Classes.Rules;
using System;
using System.Threading;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
    private TimeSpanSinceLastCallRule _timeSpanRule;
    private FixedNumberRule _usRule;
    private TimeSpanSinceLastCallRule _euRule;
    private GeographicConditionalRule _geoRule;

    [SetUp]
    public void Setup()
    {
        // Clear MemoryStore before each test
        MemoryStore.Requests.Clear();
        _timeSpanRule = new TimeSpanSinceLastCallRule(TimeSpan.FromSeconds(1));
        _usRule = new FixedNumberRule(2, TimeSpan.FromMinutes(1));
        _euRule = new TimeSpanSinceLastCallRule(TimeSpan.FromSeconds(1));
        Func<string, string> getRegion = token => token.StartsWith("US") ? "US" : "EU";

        _geoRule = new GeographicConditionalRule(_usRule, _euRule, getRegion);
    }

    [Test]
    public void FixedNumberRule_AllowsRequestUnderLimit()
    {
        var rule = new FixedNumberRule(5, TimeSpan.FromMinutes(1));
        bool result = rule.IsRequestAllowed("token1", "resource1");
        Assert.That(result, Is.True);
    }

    [Test]
    public void FixedNumberRule_BlocksRequestOverLimit()
    {
        var rule = new FixedNumberRule(1, TimeSpan.FromMinutes(1));
        rule.IsRequestAllowed("token1", "resource1");
        bool result = rule.IsRequestAllowed("token1", "resource1");
        Assert.That(result, Is.False);
    }

    [Test]
    public void RateLimitManager_AllowsRequestWithNoRules()
    {
        var manager = new RateLimitManager();
        bool result = manager.IsRequestAllowed("token1", "resource1");
        Assert.That(result, Is.True);
    }

    [Test]
    public void FixedNumberRule_ShouldAllow_ThreeRequestsInLimit()
    {
        var rule = new FixedNumberRule(3, TimeSpan.FromMinutes(1));
        var token = "user1";
        var resource = "api/resource";

        // First request should pass
        Assert.That(rule.IsRequestAllowed(token, resource), Is.True);

        // Second request should also pass
        Assert.That(rule.IsRequestAllowed(token, resource), Is.True);

        // Third request should also pass
        Assert.That(rule.IsRequestAllowed(token, resource), Is.True);
    }

    [Test]
    public void RateLimitManager_EnforcesMultipleRules()
    {
        var manager = new RateLimitManager();
        var shortRule = new FixedNumberRule(1, TimeSpan.FromSeconds(1));  // Short window for test speed

        manager.AddRule("resource1", shortRule);

        Assert.That(manager.IsRequestAllowed("token1", "resource1"), Is.True, "First request should pass.");
        Thread.Sleep(1100);  // Wait for more than 1 second
        Assert.That(manager.IsRequestAllowed("token1", "resource1"), Is.True, "Second request should pass after window reset.");
        Assert.That(manager.IsRequestAllowed("token1", "resource1"), Is.False,"Second request should fail.");
    }

    [Test]
    public void ShouldAllow_FirstRequest()
    {
        var result = _timeSpanRule.IsRequestAllowed("token1", "resource1");
        Assert.That(result, Is.True);
    }

    [Test]
    public void ShouldBlock_RequestIfWithinTimeSpan()
    {
        _timeSpanRule.IsRequestAllowed("token1", "resource1"); // First request
        Thread.Sleep(900); // Wait slightly more than 1 second
        var result = _timeSpanRule.IsRequestAllowed("token1", "resource1"); // Second request immediately
        Assert.That(result, Is.False);
    }

    [Test]
    public void ShouldApply_USRule_ForUSTokens()
    {
        Assert.That(_geoRule.IsRequestAllowed("US123", "resource1"), Is.True); // First request
        Thread.Sleep(1100); // Ensure passing the minimum time span
        Assert.That(_geoRule.IsRequestAllowed("US123", "resource1"), Is.True); // Second request
        Assert.That(_geoRule.IsRequestAllowed("US123", "resource1"), Is.False); // Third request
    }

    [Test]
    public void ShouldApply_EURule_ForEUTokens()
    {
        Assert.That(_geoRule.IsRequestAllowed("EU123", "resource1"), Is.True); // First request
        Thread.Sleep(1100); // Ensure passing the minimum time span
        Assert.That(_geoRule.IsRequestAllowed("EU123", "resource1"), Is.True); // Second request
        Assert.That(_geoRule.IsRequestAllowed("EU123", "resource1"), Is.False); // Immediate third request
    }

}