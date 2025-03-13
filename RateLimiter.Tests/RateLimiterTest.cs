using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTests
{
    [Test]
    public void FixedWindowRule_ShouldLimitRequests()
    {
        var rule = new FixedWindowRule(2, TimeSpan.FromSeconds(5));

        Assert.IsTrue(rule.IsRequestAllowed("client1").IsAllowed);
        Assert.IsTrue(rule.IsRequestAllowed("client1").IsAllowed);
        Assert.IsFalse(rule.IsRequestAllowed("client1").IsAllowed);
    }

    [Test]
    public void SlidingWindowRule_ShouldLimitRequests()
    {
        var rule = new SlidingWindowRule(3, TimeSpan.FromSeconds(5));

        Assert.IsTrue(rule.IsRequestAllowed("client1").IsAllowed);
        Assert.IsTrue(rule.IsRequestAllowed("client1").IsAllowed);
        Assert.IsTrue(rule.IsRequestAllowed("client1").IsAllowed);
        Assert.IsFalse(rule.IsRequestAllowed("client1").IsAllowed);
    }

    [Test]
    public void RateLimiterManager_ShouldApplyMultipleRules()
    {
        var manager = new RateLimiterManager(new List<ResourceRateLimitConfig>
        {
            new ResourceRateLimitConfig
            {
                Resource = "/api/test",
                Rules = new List<IRateLimitRule>
                {
                    new FixedWindowRule(2, TimeSpan.FromSeconds(5)),
                    new SlidingWindowRule(1, TimeSpan.FromSeconds(2))
                }
            }
        });

        Assert.IsTrue(manager.IsRequestAllowed("client1", "/api/test").IsAllowed);
        Assert.IsFalse(manager.IsRequestAllowed("client1", "/api/test").IsAllowed);
    }

    [Test]
    public void MultipleClients_ShouldBeTrackedIndependently()
    {
        var rule = new FixedWindowRule(2, TimeSpan.FromSeconds(5));

        Assert.IsTrue(rule.IsRequestAllowed("client1").IsAllowed);
        Assert.IsTrue(rule.IsRequestAllowed("client2").IsAllowed);
        Assert.IsTrue(rule.IsRequestAllowed("client1").IsAllowed);
        Assert.IsFalse(rule.IsRequestAllowed("client1").IsAllowed);
        Assert.IsTrue(rule.IsRequestAllowed("client2").IsAllowed);
    }

    [Test]
    public void MultipleResources_ShouldHaveSeparateLimits()
    {
        var manager = new RateLimiterManager(new List<ResourceRateLimitConfig>
        {
            new ResourceRateLimitConfig
            {
                Resource = "/api/resource1",
                Rules = new List<IRateLimitRule> { new FixedWindowRule(2, TimeSpan.FromSeconds(5)) }
            },
            new ResourceRateLimitConfig
            {
                Resource = "/api/resource2",
                Rules = new List<IRateLimitRule> { new FixedWindowRule(1, TimeSpan.FromSeconds(5)) }
            }
        });

        Assert.IsTrue(manager.IsRequestAllowed("client1", "/api/resource1").IsAllowed);
        Assert.IsTrue(manager.IsRequestAllowed("client1", "/api/resource1").IsAllowed);
        Assert.IsFalse(manager.IsRequestAllowed("client1", "/api/resource1").IsAllowed);

        Assert.IsTrue(manager.IsRequestAllowed("client1", "/api/resource2").IsAllowed);
        Assert.IsFalse(manager.IsRequestAllowed("client1", "/api/resource2").IsAllowed);
    }

    [Test]
    public async Task RequestsShouldResetAfterTimeWindow()
    {
        var rule = new FixedWindowRule(1, TimeSpan.FromMilliseconds(500));

        Assert.IsTrue(rule.IsRequestAllowed("client1").IsAllowed);
        Assert.IsFalse(rule.IsRequestAllowed("client1").IsAllowed);

        await Task.Delay(600);

        Assert.IsTrue(rule.IsRequestAllowed("client1").IsAllowed);
    }

    [Test]
    public void FixedAndSlidingWindowTogether_ShouldApplyStricterRule()
    {
        var manager = new RateLimiterManager(new List<ResourceRateLimitConfig>
        {
            new ResourceRateLimitConfig
            {
                Resource = "/api/strict",
                Rules = new List<IRateLimitRule>
                {
                    new FixedWindowRule(3, TimeSpan.FromSeconds(10)),
                    new SlidingWindowRule(1, TimeSpan.FromSeconds(2))
                }
            }
        });

        Assert.IsTrue(manager.IsRequestAllowed("client1", "/api/strict").IsAllowed);
        Assert.IsFalse(manager.IsRequestAllowed("client1", "/api/strict").IsAllowed);
    }

    [Test]
    public async Task SlidingWindowRule_ShouldAllowNewRequestsAfterOldExpire()
    {
        var rule = new SlidingWindowRule(3, TimeSpan.FromSeconds(5));

        Assert.IsTrue(rule.IsRequestAllowed("client1").IsAllowed);
        Assert.IsTrue(rule.IsRequestAllowed("client1").IsAllowed);
        Assert.IsTrue(rule.IsRequestAllowed("client1").IsAllowed);
        Assert.IsFalse(rule.IsRequestAllowed("client1").IsAllowed);

        await Task.Delay(5100);

        Assert.IsTrue(rule.IsRequestAllowed("client1").IsAllowed);
    }

    [Test]
    public void InsanelyHighRequestVolume_ShouldFailQuickly()
    {
        var rule = new FixedWindowRule(10, TimeSpan.FromSeconds(1));

        for (int i = 0; i < 10; i++)
        {
            Assert.IsTrue(rule.IsRequestAllowed("crazy_user_1").IsAllowed);
        }

        Assert.IsFalse(rule.IsRequestAllowed("crazy_user_1").IsAllowed);
    }

    [Test]
    public async Task TimeWindowBoundary_ShouldResetExactlyOnTime()
    {
        var rule = new FixedWindowRule(2, TimeSpan.FromMilliseconds(500));

        Assert.IsTrue(rule.IsRequestAllowed("boundary_user").IsAllowed);
        Assert.IsTrue(rule.IsRequestAllowed("boundary_user").IsAllowed);
        Assert.IsFalse(rule.IsRequestAllowed("boundary_user").IsAllowed);

        await Task.Delay(500);

        Assert.IsTrue(rule.IsRequestAllowed("boundary_user").IsAllowed);
    }

    [Test]
    public void MultipleUsersAndIPs_ShouldNotInterfere()
    {
        var rule = new FixedWindowRule(5, TimeSpan.FromSeconds(1));

        var users = new List<string>();
        for (int i = 0; i < 1000; i++) 
        {
            users.Add($"User{i}_IP_192.168.1.{i % 255}");
        }

        foreach (var user in users)
        {
            Assert.IsTrue(rule.IsRequestAllowed(user).IsAllowed);
        }

        Assert.IsTrue(rule.IsRequestAllowed("User999_IP_192.168.1.1").IsAllowed);
    }

    [Test]
    public void MultipleRulesConflict_ShouldEnforceStrictestRule()
    {
        var manager = new RateLimiterManager(new List<ResourceRateLimitConfig>
        {
            new ResourceRateLimitConfig
            {
                Resource = "/api/conflict",
                Rules = new List<IRateLimitRule>
                {
                    new FixedWindowRule(10, TimeSpan.FromSeconds(10)),
                    new SlidingWindowRule(2, TimeSpan.FromSeconds(5))
                }
            }
        });

        var clientKey = "user1_IP_192.168.1.1:/api/conflict";

        Assert.IsTrue(manager.IsRequestAllowed(clientKey, "/api/conflict").IsAllowed);
        Assert.IsTrue(manager.IsRequestAllowed(clientKey, "/api/conflict").IsAllowed);
        
        Assert.IsFalse(manager.IsRequestAllowed(clientKey, "/api/conflict").IsAllowed);
    }

    [Test]
    public void RandomizedClientsAndEndpoints_ShouldAllBeTrackedSeparately()
    {
        var rule = new FixedWindowRule(3, TimeSpan.FromSeconds(5));

        var random = new Random();
        var clients = new HashSet<string>();

        for (int i = 0; i < 500; i++) 
        {
            var user = $"User{random.Next(1, 100)}";
            var ip = $"192.168.{random.Next(1, 255)}.{random.Next(1, 255)}";
            var endpoint = $"/api/{random.Next(1, 10)}";
            
            var clientKey = $"{user}_{ip}:{endpoint}";
            clients.Add(clientKey);

            Assert.IsTrue(rule.IsRequestAllowed(clientKey).IsAllowed);
        }

        Assert.AreEqual(500, clients.Count);
    }
}
