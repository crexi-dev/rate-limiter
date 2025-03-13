using NUnit.Framework;
using RateLimiter;
using RateLimiter.Rules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
    [Test]
    public async Task AllowRequest_AfterTimeWindow_ShouldResetCounter()
    {
        var rateLimiter = new RateLimiter(new List<IRateLimitRule> { new FixedWindowRule(1, TimeSpan.FromMilliseconds(500)) });

        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
        Assert.IsFalse(rateLimiter.AllowRequest("client1"));

        await Task.Delay(600);
        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
    }

    [Test]
    public void AllowRequest_WithinLimit_ShouldReturnTrue()
    {
        var rateLimiter = new RateLimiter(new List<IRateLimitRule> { new FixedWindowRule(3, TimeSpan.FromMinutes(1)) });

        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
    }

    [Test]
    public void AllowRequest_ExceedsLimit_ShouldReturnFalse()
    {
        var rateLimiter = new RateLimiter(new List<IRateLimitRule> { new FixedWindowRule(2, TimeSpan.FromMinutes(1)) });

        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
        Assert.IsFalse(rateLimiter.AllowRequest("client1"));
    }

    [Test]
    public void AllowRequest_MultipleClients_ShouldBeTrackedIndividually()
    {
        var rateLimiter = new RateLimiter(new List<IRateLimitRule> { new FixedWindowRule(2, TimeSpan.FromMinutes(1)) });

        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
        Assert.IsTrue(rateLimiter.AllowRequest("client2"));
        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
        Assert.IsFalse(rateLimiter.AllowRequest("client1"));
        Assert.IsTrue(rateLimiter.AllowRequest("client2"));
    }

    [Test]
    public void AllowRequest_MultipleRules_AllRulesMustPass()
    {
        var rateLimiter = new RateLimiter(new List<IRateLimitRule>
        {
            new FixedWindowRule(2, TimeSpan.FromMinutes(1)), // Max 2 per minute
            new FixedWindowRule(1, TimeSpan.FromSeconds(10)) // Max 1 per 10 sec
        });

        Assert.IsTrue(rateLimiter.AllowRequest("client1")); // First rule allows
        Assert.IsFalse(rateLimiter.AllowRequest("client1")); // Second rule blocks
    }

    [Test]
    public async Task AllowRequest_BurstRequests_ShouldBeBlocked()
    {
        var rateLimiter = new RateLimiter(new List<IRateLimitRule> { new FixedWindowRule(3, TimeSpan.FromSeconds(1)) });

        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
        Assert.IsFalse(rateLimiter.AllowRequest("client1")); // 4th request should be blocked immediately

        await Task.Delay(1100); // Wait for time window to reset
        Assert.IsTrue(rateLimiter.AllowRequest("client1")); // Should be allowed after reset
    }

    [Test]
    public void AllowRequest_ConcurrentClients_ShouldNotInterfere()
    {
        var rateLimiter = new RateLimiter(new List<IRateLimitRule> { new FixedWindowRule(2, TimeSpan.FromSeconds(1)) });

        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
        Assert.IsTrue(rateLimiter.AllowRequest("client2"));
        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
        Assert.IsFalse(rateLimiter.AllowRequest("client1")); // Blocked for client1, still open for client2
        Assert.IsTrue(rateLimiter.AllowRequest("client2")); // Client2 should still be allowed
    }

    [Test]
    public async Task AllowRequest_ShortTimeWindow_ShouldResetQuickly()
    {
        var rateLimiter = new RateLimiter(new List<IRateLimitRule> { new FixedWindowRule(1, TimeSpan.FromMilliseconds(200)) });

        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
        Assert.IsFalse(rateLimiter.AllowRequest("client1"));

        await Task.Delay(250); // Wait beyond the reset window
        Assert.IsTrue(rateLimiter.AllowRequest("client1"));
    }
}
