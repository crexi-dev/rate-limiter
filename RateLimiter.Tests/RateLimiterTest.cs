using NUnit.Framework;
using System;
using System.Collections.Generic;

[TestFixture]
public class RateLimiterTest
{
    private RateLimiter _rateLimiter;
    private const string Resource = "resource1";

    [SetUp]
    public void SetUp()
    {
        _rateLimiter = new RateLimiter();
    }

    [Test]
    public void FixedWindowRateLimitRule_AllowsRequestsWithinLimit()
    {
        var fixedRule = new FixedWindowRateLimitRule(5, TimeSpan.FromMinutes(1));
        _rateLimiter.AddRule(Resource, fixedRule);

        string client = "Client1";

        for (int i = 0; i < 5; i++)
        {
            Assert.IsTrue(_rateLimiter.HandleRequest(client, Resource));
        }
    }

    [Test]
    public void FixedWindowRateLimitRule_DeniesRequestsExceedingLimit()
    {
        var fixedRule = new FixedWindowRateLimitRule(5, TimeSpan.FromMinutes(1));
        _rateLimiter.AddRule(Resource, fixedRule);

        string client = "Client1";

        for (int i = 0; i < 5; i++)
        {
            Assert.IsTrue(_rateLimiter.HandleRequest(client, Resource));
        }

        // The 6th request should be denied
        Assert.IsFalse(_rateLimiter.HandleRequest(client, Resource));
    }

    [Test]
    public void SlidingWindowRateLimitRule_AllowsRequestAfterInterval()
    {
        var slidingRule = new SlidingWindowRateLimitRule(TimeSpan.FromSeconds(1));
        _rateLimiter.AddRule(Resource, slidingRule);

        string client = "Client1";

        Assert.IsTrue(_rateLimiter.HandleRequest(client, Resource));

        // Wait for the interval to pass
        System.Threading.Thread.Sleep(1100);

        Assert.IsTrue(_rateLimiter.HandleRequest(client, Resource));
    }

    [Test]
    public void SlidingWindowRateLimitRule_DeniesRequestWithinInterval()
    {
        var slidingRule = new SlidingWindowRateLimitRule(TimeSpan.FromSeconds(5));
        _rateLimiter.AddRule(Resource, slidingRule);

        string client = "Client1";

        Assert.IsTrue(_rateLimiter.HandleRequest(client, Resource));
        Assert.IsFalse(_rateLimiter.HandleRequest(client, Resource));
    }

    [Test]
    public void RegionBasedRateLimitRule_AppliesUSRule()
    {
        var regionRules = new Dictionary<string, IRateLimitRule>
        {
            { "US", new FixedWindowRateLimitRule(2, TimeSpan.FromMinutes(1)) },
            { "EU", new SlidingWindowRateLimitRule(TimeSpan.FromSeconds(10)) }
        };
        var regionRule = new RegionBasedRateLimitRule(regionRules);
        _rateLimiter.AddRule(Resource, regionRule);

        string client = "USClient123";

        Assert.IsTrue(_rateLimiter.HandleRequest(client, Resource));
        Assert.IsTrue(_rateLimiter.HandleRequest(client, Resource));
        Assert.IsFalse(_rateLimiter.HandleRequest(client, Resource));
    }

    [Test]
    public void RegionBasedRateLimitRule_AppliesEURule()
    {
        var regionRules = new Dictionary<string, IRateLimitRule>
        {
            { "US", new FixedWindowRateLimitRule(2, TimeSpan.FromMinutes(1)) },
            { "EU", new SlidingWindowRateLimitRule(TimeSpan.FromSeconds(2)) }
        };
        var regionRule = new RegionBasedRateLimitRule(regionRules);
        _rateLimiter.AddRule(Resource, regionRule);

        string client = "EUClient123";

        Assert.IsTrue(_rateLimiter.HandleRequest(client, Resource));
        System.Threading.Thread.Sleep(2100); // Wait for the interval to pass
        Assert.IsTrue(_rateLimiter.HandleRequest(client, Resource));
    }
}
