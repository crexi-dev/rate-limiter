using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using RateLimiter.Rules;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
    [Test]
    public void Example()
    {
        Assert.That(true, Is.True);
    }

    [Test]
    public void Test()
    {
       
    }

    [Test]
    public void NoRulesAtAll_Ok()
    {
        // arrange
        var rl = new RateLimiter();

        // act
        var result = rl.TryRequest("token1");

        //assert
        Assert.True(result);
    }

    [Test]
    public void NoRulesForToken_Ok()
    {
        // arrange
        var rules = new Dictionary<string, List<IRule>>
        {
            { "token1", new List<IRule>{new RestrictionRule()} }
        };
        var rl = new RateLimiter(rules);

        // act
        var result = rl.TryRequest("token2");

        //assert
        Assert.True(result);
    }
    
    [Test]
    public void AllRequestsAreProhibited_Ok()
    {
        // arrange
        var rules = new Dictionary<string, List<IRule>>
        {
            { "token1", new List<IRule>{new RestrictionRule()} }
        };
        var rl = new RateLimiter(rules);

        // act
        var result = rl.TryRequest("token1");
        var result2 = rl.TryRequest("token1");
        var result3 = rl.TryRequest("token1");
        var result4 = rl.TryRequest("token1");

        //assert
        Assert.False(result);
        Assert.False(result2);
        Assert.False(result3);
        Assert.False(result4);
    }

    [Test]
    public void RequestPerMinute_ThreeOk_OneOutOfCount()
    {
        // arrange
        var rules = new Dictionary<string, List<IRule>>
        {
            { "token1", new List<IRule>{new IntervalRule(3, 60)} }
        };
        var rl = new RateLimiter(rules);

        // act
        var result = rl.TryRequest("token1");
        Thread.Sleep(15000);
        var result2 = rl.TryRequest("token1");
        Thread.Sleep(15000);
        var result3 = rl.TryRequest("token1");
        Thread.Sleep(15000);
        var result4 = rl.TryRequest("token1");

        //assert
        Assert.True(result);
        Assert.True(result2);
        Assert.True(result3);
        Assert.False(result4);
    }
    
    [Test]
    [TestCase(1, 1)]
    public void DelayRule_Fail(int interval, int count)
    {
        // arrange
        var rules = new Dictionary<string, List<IRule>>
        {
            { "token1", new List<IRule>{new DelayRule(30)} }
        };
        var rl = new RateLimiter(rules);

        // act
        var result = rl.TryRequest("token1");
        Thread.Sleep(10000);
        var result2 = rl.TryRequest("token1");

        //assert
        Assert.True(result);
        Assert.False(result2);
    }

    [Test]
    public void ManyRequests()
    {
        // arrange
        var rules = new Dictionary<string, List<IRule>>
        {
            { "token1", new List<IRule>{new DelayRule(20)}},
            { "token2", new List<IRule>{new RestrictionRule()} },
            { "token3", new List<IRule>{new IntervalRule(2, 20)} }
        };
        var rl = new RateLimiter(rules);

        // act
        var result = rl.TryRequest("token2");
        var result2 = rl.TryRequest("token1");
        Thread.Sleep(10000);
        var result3 = rl.TryRequest("token3");
        Thread.Sleep(10000);
        var result4 = rl.TryRequest("token3");
        Thread.Sleep(10000);
        var result5 = rl.TryRequest("token1");
        var result6 = rl.TryRequest("token1");
        var result7 = rl.TryRequest("token2");

        //assert
        Assert.False(result);
        Assert.True(result2);
        Assert.True(result3);
        Assert.True(result4);
        Assert.True(result5);
        Assert.False(result6);
        Assert.False(result7);
    }
}