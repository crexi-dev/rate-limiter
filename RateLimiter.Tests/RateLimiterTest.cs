using NUnit.Framework;
using System.Threading.Tasks;
using System;

namespace RateLimiter.Tests;


[TestFixture]
public class RateLimiterTest
{
    private Core.Services.RateLimiter _rateLimiter;

    [SetUp]
    public void Setup()
    {
        _rateLimiter = new Core.Services.RateLimiter(5, TimeSpan.FromSeconds(1));
    }

    [Test]
    public async Task ShouldAllowRequestsWithinLimit()
    {
        var accessToken = "user123";
        var resource1 = "user123";

        for (int i = 0; i < 5; i++)
        {
            Assert.IsTrue(_rateLimiter.IsRequestAllowed(accessToken, resource1));
            await Task.Delay(200);
        }
    }

    [Test]
    public async Task ShouldDenyRequestsBeyondLimit()
    {
        var accessToken = "user123";
        var resource1 = "user123";

        for (int i = 0; i < 5; i++)
        {
            _rateLimiter.IsRequestAllowed(accessToken, resource1);
        }

        Assert.IsFalse(_rateLimiter.IsRequestAllowed(accessToken, resource1));
    }


    [Test]
    public async Task ShouldAllowRequestsAfterTimePeriod()
    {
        var accessToken = "user123";
        var resource1 = "user123";
        var resource2 = "user124";

        for (int i = 0; i < 5; i++)
        {
            _rateLimiter.IsRequestAllowed(accessToken, resource1);
        }
        _rateLimiter.IsRequestAllowed(accessToken, resource2);

        Assert.IsFalse(_rateLimiter.IsRequestAllowed(accessToken, resource1));
        Assert.IsTrue(_rateLimiter.IsRequestAllowed(accessToken, resource2));

        await Task.Delay(1000);

        Assert.IsTrue(_rateLimiter.IsRequestAllowed(accessToken, resource1));
    }

}