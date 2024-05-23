using System;
using NUnit.Framework;
using RateLimiter.Rules;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTests
{
    private string clientToken1;
    private string clientToken2;
    private string clientToken3;
    private string clientTokenUS;
    private string clientTokenEU;
    private DateTime requestTime;
    private RateLimiter.Services.RateLimiter rateLimiter;

    [SetUp]
    public void Setup()
    {
        clientToken1 = "client1";
        clientToken2 = "client2";
        clientToken3 = "client3";
        clientTokenUS = "clientUS";
        clientTokenEU = "clientEU";
        requestTime = DateTime.UtcNow;
        rateLimiter = new Services.RateLimiter();
    }

    [Test]
    public void XRequestsPerTimespanRule_AllowsRequestsWithinLimit()
    {
        var rule = new XRequestsPerTimespanRule(3, TimeSpan.FromMinutes(1));
        rateLimiter.AddRule("resource1", rule);

        Assert.IsTrue(rateLimiter.IsRequestAllowed(clientToken1, "resource1", requestTime, string.Empty));
        Assert.IsTrue(rateLimiter.IsRequestAllowed(clientToken1, "resource1", requestTime.AddSeconds(10),
            string.Empty));
        Assert.IsTrue(rateLimiter.IsRequestAllowed(clientToken1, "resource1", requestTime.AddSeconds(20),
            string.Empty));
        Assert.IsFalse(rateLimiter.IsRequestAllowed(clientToken1, "resource1", requestTime.AddSeconds(30),
            string.Empty));
    }

    [Test]
    public void CertainTimespanPassedRule_RestrictsRequestsBasedOnTimespan()
    {
        var rule = new CertainTimespanPassedRule(TimeSpan.FromMinutes(1));
        rateLimiter.AddRule("resource2", rule);

        Assert.IsTrue(rateLimiter.IsRequestAllowed(clientToken2, "resource2", requestTime, string.Empty));
        Assert.IsFalse(rateLimiter.IsRequestAllowed(clientToken2, "resource2", requestTime.AddSeconds(30),
            string.Empty));
        Assert.IsTrue(rateLimiter.IsRequestAllowed(clientToken2, "resource2", requestTime.AddMinutes(1),
            string.Empty));
    }

    [Test]
    public void CombinedRules_WorkCorrectly()
    {
        var xRequestsRule = new XRequestsPerTimespanRule(2, TimeSpan.FromMinutes(1));
        var certainTimespanRule = new CertainTimespanPassedRule(TimeSpan.FromSeconds(30));
        rateLimiter.AddRule("resource3", xRequestsRule);
        rateLimiter.AddRule("resource3", certainTimespanRule);

        Assert.IsTrue(rateLimiter.IsRequestAllowed(clientToken3, "resource3", requestTime, string.Empty));
        Assert.IsFalse(rateLimiter.IsRequestAllowed(clientToken3, "resource3", requestTime.AddSeconds(20),
            string.Empty));
        Assert.IsFalse(rateLimiter.IsRequestAllowed(clientToken3, "resource3", requestTime.AddSeconds(40),
            string.Empty));
        Assert.IsTrue(rateLimiter.IsRequestAllowed(clientToken3, "resource3",
            requestTime.AddMinutes(1).AddSeconds(20), string.Empty));
        Assert.IsFalse(rateLimiter.IsRequestAllowed(clientToken3, "resource3",
            requestTime.AddMinutes(1).AddSeconds(40), string.Empty));
    }

    [Test]
    public void RegionBasedRules_WorkCorrectly()
    {
        var usRegionRule = new XRequestsPerTimespanRule(2, TimeSpan.FromMinutes(1));
        var euRegionRule = new CertainTimespanPassedRule(TimeSpan.FromSeconds(30));

        rateLimiter.AddRegionRule("US", usRegionRule);
        rateLimiter.AddRegionRule("EU", euRegionRule);

        // US Region Tests
        Assert.IsTrue(rateLimiter.IsRequestAllowed(clientTokenUS, "resource", requestTime, "US"));
        Assert.IsTrue(rateLimiter.IsRequestAllowed(clientTokenUS, "resource", requestTime.AddSeconds(10), "US"));
        Assert.IsFalse(rateLimiter.IsRequestAllowed(clientTokenUS, "resource", requestTime.AddSeconds(20), "US"));

        // EU Region Tests
        Assert.IsTrue(rateLimiter.IsRequestAllowed(clientTokenEU, "resource", requestTime, "EU"));
        Assert.IsFalse(rateLimiter.IsRequestAllowed(clientTokenEU, "resource", requestTime.AddSeconds(20), "EU"));
        Assert.IsTrue(rateLimiter.IsRequestAllowed(clientTokenEU, "resource", requestTime.AddSeconds(40), "EU"));
    }
}