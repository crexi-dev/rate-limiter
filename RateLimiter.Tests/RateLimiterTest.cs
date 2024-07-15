using NUnit.Framework;
using RateLimiter.RateLimitRules;
using System;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
    private RateLimitRuleManager _rateLimiter;

    [SetUp]
    public void Setup()
    {
        _rateLimiter = new RateLimitRuleManager();
    }

    [Test]
    public void FixedWindowRateLimitRule_Should_Enforce_Limit()
    {
        // Configure rule to allow 5 request per minute
        var rule = new FixedWindowRateLimitRule(5, TimeSpan.FromMinutes(1));
        _rateLimiter.AddRule("/api/test", rule);

        var clientIdentifier = Guid.NewGuid().ToString();
        var endpoint = "/api/test";

        // First 5 requests should be allowed
        for (int i = 0; i < 5; i++)
        {
            Assert.IsFalse(_rateLimiter.IsLimitExceeded(endpoint, clientIdentifier));
            _rateLimiter.RecordRequest(endpoint, clientIdentifier);
        }

        // The rest of requests should be rate limited. trying 2 requests below:
        Assert.IsTrue(_rateLimiter.IsLimitExceeded(endpoint, clientIdentifier));
        Assert.IsTrue(_rateLimiter.IsLimitExceeded(endpoint, clientIdentifier));
    }

    [Test]
    public void SlidingWindowRateLimitRule_Should_Enforce_Limit()
    {
        // Configure rule to allow only 3 request in a span of 10 seconds
        var rule = new SlidingWindowRateLimitRule(3, TimeSpan.FromSeconds(10));
        _rateLimiter.AddRule("/api/test", rule);

        var clientIdentifier = Guid.NewGuid().ToString();
        var endpoint = "/api/test";

        // Simulate 3 requests within the window
        for (int i = 0; i < 3; i++)
        {
            Assert.IsFalse(_rateLimiter.IsLimitExceeded(endpoint, clientIdentifier));
            _rateLimiter.RecordRequest(endpoint, clientIdentifier);
        }

        // The 4th request within the window should be rate limited
        Assert.IsTrue(_rateLimiter.IsLimitExceeded(endpoint, clientIdentifier));
    }

    [Test]
    public void SlidingWindowRateLimitRule_Should_Allow_After_Window_Expires()
    {
        var rule = new SlidingWindowRateLimitRule(3, TimeSpan.FromSeconds(1));
        _rateLimiter.AddRule("/api/test", rule);

        var clientIdentifier = "client1";
        var endpoint = "/api/test";

        // Simulate 3 requests within the window
        for (int i = 0; i < 3; i++)
        {
            Assert.IsFalse(_rateLimiter.IsLimitExceeded(endpoint, clientIdentifier));
            _rateLimiter.RecordRequest(endpoint, clientIdentifier);
        }

        // The 4th request within the window should be rate limited
        Assert.IsTrue(_rateLimiter.IsLimitExceeded(endpoint, clientIdentifier));

        // Wait for the window to expire
        System.Threading.Thread.Sleep(1100);

        // New requests should be allowed after the window expires
        Assert.IsFalse(_rateLimiter.IsLimitExceeded(endpoint, clientIdentifier));
        _rateLimiter.RecordRequest(endpoint, clientIdentifier);
    }
}