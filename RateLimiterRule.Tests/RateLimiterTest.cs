using System.Threading.Tasks;
using System;
using Xunit;
using RateLimiter.Rules;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using RateLimiter;

namespace RateLimiterRule.Tests;


public class RateLimiterTest
{

    [Fact]
    public void ProcessRequests_ReturnsTrue_WhenNoRulesConfigured()
    {
        var rateLimiter = new RateLimiter.RateLimiter();
        var client = new Client(Guid.NewGuid());

        var result = rateLimiter.ProcessRequests(client, "myResource");

        Assert.True(result);
    }

    [Theory]
    [InlineData(2, 1, 5, 1, true)]
    [InlineData(2, 1, 5, 0.2, false)]
    public void ProcessRequests_ReturnsExpectedResult(int limit, double limitSecond, int numRequests, double delay, bool expectedResult)
    {
        var rateLimiter = new RateLimiter.RateLimiter();
        var client = new Client(Guid.NewGuid());
        var rule = new PerTimespanRule(limit, TimeSpan.FromSeconds(limitSecond));
        rateLimiter.AddRule("myResource", rule);
        List<bool> responses = new List<bool>();
        for (int i = 0; i < numRequests; i++)
        {
            responses.Add(rateLimiter.ProcessRequests(client, "myResource"));
            Task.Delay(TimeSpan.FromSeconds(delay)).Wait();
        }

        Assert.True(expectedResult ? responses.All(x => x == expectedResult) 
                                   : responses.Any(x => x == expectedResult));
    }

    [Fact]
    public void TestTwoParallelUsers()
    {
        var limiter = new RateLimiter.RateLimiter();
        limiter.AddRule("myResource", new SinceLastCallRule(TimeSpan.FromSeconds(0.5)));
        var client1 = new Client(Guid.NewGuid());
        var client2 = new Client(Guid.NewGuid());

        var results = new ConcurrentBag<bool>();
        Parallel.Invoke(
            () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    results.Add(limiter.ProcessRequests(client1, "myResource"));
                    Task.Delay(1000).Wait(); // Wait 1 second
                }
            },
            () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    results.Add(limiter.ProcessRequests(client2, "myResource"));
                    Task.Delay(1000).Wait(); // Wait 1 second
                }
            }
            );

        Assert.True(results.All(r => r)); // All requests should be allowed
    }

    [Fact]
    public void TestMultipleRules()
    {
        var limiter = new RateLimiter.RateLimiter();
        limiter.AddRule("myResource", new SinceLastCallRule(TimeSpan.FromSeconds(5)));
        limiter.AddRule("myResource", new PerTimespanRule(2, TimeSpan.FromSeconds(10)));
        var client = new Client(Guid.NewGuid());

        var results = new List<bool>();
        results.Add(limiter.ProcessRequests(client, "myResource")); // true
        results.Add(limiter.ProcessRequests(client, "myResource")); // false
        results.Add(limiter.ProcessRequests(client, "myResource")); // false
        
        Thread.Sleep(7000); 
        
        results.Add(limiter.ProcessRequests(client, "myResource")); // true

        Assert.True(results[0]);
        Assert.False(results[1]);
        Assert.False(results[2]);
        Assert.True(results[3]);
    }
}