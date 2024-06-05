using NUnit.Framework;
using System;
using System.Threading;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
    [Test]
    public void RateLimiter_AllowsRequestsWithinConfiguredLimits()
    {
        var rateLimiter = new RateLimiter();

        var dailyRule = new DailyLimitRule(100);
        var burstRule = new BurstLimitRule(10, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(10));
        var timeOfDayRule = new TimeOfDayLimitRule(TimeSpan.FromHours(9), TimeSpan.FromHours(20));
        var userLevelRule = new UserLevelRateLimitRule(GetUserLevel);
        var ipAddressRule = new IPAddressRateLimitRule(20, TimeSpan.FromMinutes(10), GetClientIPAddress);

        userLevelRule.ConfigureUserLevel("free", new RequestCountPerTimespanRule(5, TimeSpan.FromMinutes(1)));
        userLevelRule.ConfigureUserLevel("premium", new RequestCountPerTimespanRule(50, TimeSpan.FromMinutes(1)));

        rateLimiter.ConfigureResource("api/resource1", dailyRule);
        rateLimiter.ConfigureResource("api/resource2", burstRule);
        rateLimiter.ConfigureResource("api/resource3", timeOfDayRule);
        rateLimiter.ConfigureResource("api/resource4", userLevelRule);
        rateLimiter.ConfigureResource("api/resource5", ipAddressRule);

        var clientId = "premium_customer_1";

        for (int i = 0; i < 10; i++)
        {
            Assert.IsTrue(rateLimiter.AllowRequest("api/resource1", clientId), $"Request to resource1 at iteration {i} should be allowed.");
            Assert.IsTrue(rateLimiter.AllowRequest("api/resource2", clientId), $"Request to resource2 at iteration {i} should be allowed.");
            Assert.IsTrue(rateLimiter.AllowRequest("api/resource3", clientId), $"Request to resource3 at iteration {i} should be allowed.");
            Assert.IsTrue(rateLimiter.AllowRequest("api/resource4", clientId), $"Request to resource4 at iteration {i} should be allowed.");
            Assert.IsTrue(rateLimiter.AllowRequest("api/resource5", clientId), $"Request to resource5 at iteration {i} should be allowed.");
        }

        for (int i = 0; i < 90; i++)
        {
            rateLimiter.AllowRequest("api/resource1", clientId);
        }
        bool finalRequestResult = rateLimiter.AllowRequest("api/resource1", clientId);
        Assert.IsFalse(finalRequestResult, "Request to resource1 after 100 requests should be denied.");
    }

    private string GetUserLevel(string clientId)
    {
        return clientId.StartsWith("premium") ? "premium" : "free";
    }

    private string GetClientIPAddress(string clientId)
    {
        return clientId.StartsWith("client") ? "192.168.0.1" : "10.0.0.1";
    }
}