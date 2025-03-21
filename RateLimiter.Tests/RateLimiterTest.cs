using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
    private ApiRateLimiter _rateLimiter;

    [SetUp]
    public void Setup()
    {
        _rateLimiter = new ApiRateLimiter(maxRequests: 3, timeWindowSeconds: 30);
    }

    [Test]
    public void AllowsRequestsWithinLimit()
    {
        Assert.That(_rateLimiter.IsAllowed("client1"), Is.True);
        Assert.That(_rateLimiter.IsAllowed("client1"), Is.True);
        Assert.That(_rateLimiter.IsAllowed("client1"), Is.True);
    }

    [Test]
    public void BlocksRequestsOverLimit()
    {
        // First 3 requests should be allowed
        for (int i = 0; i < 3; i++)
        {
            Assert.That(_rateLimiter.IsAllowed("client1"), Is.True);
        }

        // Fourth request should be blocked
        Assert.That(_rateLimiter.IsAllowed("client1"), Is.False);
    }

    [Test]
    public async Task AllowsRequestsAfterTimeWindowReset()
    {
        // Use up all requests
        for (int i = 0; i < 3; i++)
        {
            Assert.That(_rateLimiter.IsAllowed("client1"), Is.True);
        }

        // Wait for time window to reset
        await Task.Delay(TimeSpan.FromSeconds(30));

        // Should allow requests again
        Assert.That(_rateLimiter.IsAllowed("client1"), Is.True);
    }

    [Test]
    public void HandlesMultipleClientsIndependently()
    {
        // client1 uses all requests
        for (int i = 0; i < 3; i++)
        {
            Assert.That(_rateLimiter.IsAllowed("client1"), Is.True);
        }
        Assert.That(_rateLimiter.IsAllowed("client1"), Is.False);

        // client2 should still be allowed
        Assert.That(_rateLimiter.IsAllowed("client2"), Is.True);
    }

    [Test]
    public void ZeroRequestsConfiguration()
    {
        var zeroLimiter = new ApiRateLimiter(maxRequests: 0, timeWindowSeconds: 1);
        Assert.That(zeroLimiter.IsAllowed("client1"), Is.False);
    }

    [Test]
    public async Task ParallelRequests()
    {
        var tasks = new Task<bool>[5];
        for (int i = 0; i < 5; i++)
        {
            tasks[i] = Task.Run(() => _rateLimiter.IsAllowed("client1"));
        }

        var results = await Task.WhenAll(tasks);
        Assert.That(results.Count(r => r), Is.EqualTo(3));
        Assert.That(results.Count(r => !r), Is.EqualTo(2));
    }

    [Test]
    public void RequestMultipleRules()
    {
        _rateLimiter.AddRule(SampleRules.CreateDailyQuotaRule(2));

        // First request is allowed
        Assert.That(_rateLimiter.IsAllowed("client1"), Is.True);

        // Second request should be blocked
        Assert.That(_rateLimiter.IsAllowed("client1"), Is.False);
    }
    [Test]
    public void RequestWithNonDefaultRule()
    {
        _rateLimiter.RemoveAll();
        _rateLimiter.AddRule(SampleRules.CreateDailyQuotaRule(2));
        Assert.That(_rateLimiter.IsAllowed("client1"), Is.True);
        Assert.That(_rateLimiter.IsAllowed("client1"), Is.True);
        Assert.That(_rateLimiter.IsAllowed("client1"), Is.False);
    }
    [Test]
    public async Task RequestCertainTimeHasPassedAsync()
    {
        ApiRateLimiter _timeHasPassed = new ApiRateLimiter(TimeSpan.FromSeconds(30));
        Assert.That(_timeHasPassed.IsAllowed("client1"), Is.True);
        Assert.That(_timeHasPassed.IsAllowed("client1"), Is.False);

        await Task.Delay(TimeSpan.FromSeconds(30));
        
        // 30 seconds must pass before the next request is allpwed
        Assert.That(_timeHasPassed.IsAllowed("client1"), Is.True);

    }
}