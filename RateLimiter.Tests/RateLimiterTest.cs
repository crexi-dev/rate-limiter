using NUnit.Framework;
using RateLimiters;
using System;
using System.Threading;

[TestFixture]
public class RateLimiterTests
{
    private RateLimiter _rateLimiter;
    private XRequestsPerTimespanRule _rule1;
    private TimespanSinceLastCallRule _rule2;

    [SetUp]
    public void Setup()
    {
        _rule1 = new XRequestsPerTimespanRule(10, TimeSpan.FromMinutes(1));
        _rule2 = new TimespanSinceLastCallRule(TimeSpan.FromSeconds(5));

        _rateLimiter = new RateLimiter();
    }

    [Test]
    public void Test_XRequestsPerTimespanRule_AllowsRequestsUnderLimit()
    {
        var policy = new RateLimitPolicy(_rule1);
        _rateLimiter.AddPolicy("resource1", policy);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(_rateLimiter.IsRequestAllowed("client1", "resource1"), Is.EqualTo(true));
            _rateLimiter.RecordRequest("client1", "resource1");
        }
    }

    [Test]
    public void Test_XRequestsPerTimespanRule_BlocksRequestsOverLimit()
    {
        var policy = new RateLimitPolicy(_rule1);
        _rateLimiter.AddPolicy("resource1", policy);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(_rateLimiter.IsRequestAllowed("client1", "resource1"), Is.EqualTo(true));
            _rateLimiter.RecordRequest("client1", "resource1");
        }

        Assert.That(_rateLimiter.IsRequestAllowed("client1", "resource1"), Is.EqualTo(false));
    }

    [Test]
    public void Test_TimespanSinceLastCallRule_AllowsRequestAfterTimespan()
    {
        var policy = new RateLimitPolicy(_rule2);
        _rateLimiter.AddPolicy("resource2", policy);

        Assert.That(_rateLimiter.IsRequestAllowed("client1", "resource2"), Is.EqualTo(true));
        _rateLimiter.RecordRequest("client1", "resource2");

        Thread.Sleep(TimeSpan.FromSeconds(6)); // Sleep for more than the defined timespan

        Assert.That(_rateLimiter.IsRequestAllowed("client1", "resource2"), Is.EqualTo(true));
    }

    [Test]
    public void Test_TimespanSinceLastCallRule_BlocksRequestBeforeTimespan()
    {
        var policy = new RateLimitPolicy(_rule2);
        _rateLimiter.AddPolicy("resource2", policy);

        Assert.That(_rateLimiter.IsRequestAllowed("client1", "resource2"), Is.EqualTo(true));
        _rateLimiter.RecordRequest("client1", "resource2");

        Assert.That(_rateLimiter.IsRequestAllowed("client1", "resource2"), Is.EqualTo(false));
    }

    [Test]
    public void Test_CombinedRules()
    {
        var policy = new RateLimitPolicy(_rule1,_rule2);
        _rateLimiter.AddPolicy("resource3", policy);

        for (int i = 0; i < 10; i++)
        {
            Assert.That(_rateLimiter.IsRequestAllowed("client1", "resource3"), Is.EqualTo(true));
           
            _rateLimiter.RecordRequest("client1", "resource3");
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
         
        // Should block because of XRequestsPerTimespanRule
        Assert.That(_rateLimiter.IsRequestAllowed("client1", "resource3"),Is.EqualTo(false));

        // Sleep for the timespan of TimespanSinceLastCallRule
        Thread.Sleep(TimeSpan.FromSeconds(6));

        // Should still block because of XRequestsPerTimespanRule
        Assert.That(_rateLimiter.IsRequestAllowed("client1", "resource3"),Is.EqualTo(false));
        
        
    }
}
