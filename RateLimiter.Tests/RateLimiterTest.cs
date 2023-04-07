using NUnit.Framework;
using RateLimiter.Rules;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{

    [Test]
	public void UsaRuleTest_Limit_Exceeded()
	{
        var rule = new UsaRule()
        {
            Interval = TimeSpan.FromSeconds(10),
            Limit = 2
        };
        var resourceList = new List<IRule> { rule };
        
        string apiResource = Guid.NewGuid().ToString();

        var resource = new LimiterResource(apiResource, resourceList);

        var executor = new RateLimiterExecutor(resource, new RequestClientIdentifier());

        bool isExceeded = false;
        for (int i = 0; i < 3; i++)
        {
            isExceeded = executor.IsExceeded();
            if (isExceeded) 
            {
                break;
            }
        }

		Assert.That(isExceeded, Is.True);
	}

    [Test]
    public void EuRuleTest_LimitExceeded()
    {
        var rule = new EuRule()
        {
            Interval = TimeSpan.FromSeconds(10),
        };
        string apiResource = Guid.NewGuid().ToString();
        var resourceList = new List<IRule> { rule };
        var resource = new LimiterResource(apiResource, resourceList);

        var executor = new RateLimiterExecutor(resource, new RequestClientIdentifier());

        bool isExceeded = false;
        //one there more should raise limit
        for (int i = 0; i < 2; i++)
        {
            isExceeded = executor.IsExceeded();
            if (isExceeded)
            {
                break;
            }
        }

        Assert.That(isExceeded, Is.True);
    }

    [Test]
    public void UsaRuleTest_Limit_Not_Exceeded()
    {
        var rule = new UsaRule()
        {
            Interval = TimeSpan.FromSeconds(10),
            Limit = 3
        };
        var resourceList = new List<IRule> { rule };

        string apiResource = Guid.NewGuid().ToString();

        var resource = new LimiterResource(apiResource, resourceList);

        var executor = new RateLimiterExecutor(resource, new RequestClientIdentifier());

        bool isExceeded = false;
        for (int i = 0; i < 3; i++)
        {
            isExceeded = executor.IsExceeded();
            if (isExceeded)
            {
                break;
            }
        }

        Assert.That(isExceeded, Is.False);
    }

    [Test]
    public void EuRuleTestLimit_Not_Exceeded()
    {
        var rule = new EuRule()
        {
            Interval = TimeSpan.FromMilliseconds(30),
        };
        string apiResource = Guid.NewGuid().ToString();
        var resourceList = new List<IRule> { rule };
        var resource = new LimiterResource(apiResource, resourceList);

        var executor = new RateLimiterExecutor(resource, new RequestClientIdentifier());

        bool isExceeded = false;
        //one there more should raise limit
        for (int i = 0; i < 2; i++)
        {
            isExceeded = executor.IsExceeded();
            Thread.Sleep(rule.Interval);
            if (isExceeded)
            {
                break;
            }
        }

        Assert.That(isExceeded, Is.False);
    }

    [Test]
    public void Eu_Usa_RuleTestLimitExceeded()
    {
        var euRule = new EuRule()
        {
            Interval = TimeSpan.FromSeconds(30),
        };
        var usaRule = new UsaRule()
        {
            Interval = TimeSpan.FromSeconds(30),
            Limit =5
        };
        string apiResource = Guid.NewGuid().ToString();
        var resourceList = new List<IRule> { euRule, euRule };
        var resource = new LimiterResource(apiResource, resourceList);

        var executor = new RateLimiterExecutor(resource, new RequestClientIdentifier());

        bool isExceeded = false;
        //one there more should raise limit
        for (int i = 0; i < 2; i++)
        {
            isExceeded = executor.IsExceeded();
            if (isExceeded)
            {
                break;
            }
        }
        // Eu limit should exceed and affect 
        Assert.That(isExceeded, Is.True);
    }

    [Test]
    public void Eu_Usa_RuleTestLimit_Not_Exceeded()
    {
        var euRule = new EuRule()
        {
            Interval = TimeSpan.FromMinutes(30),
        };
        var usaRule = new UsaRule()
        {
            Interval = TimeSpan.FromSeconds(30),
            Limit = 4
        };
        string apiResource = Guid.NewGuid().ToString();
        var resourceList = new List<IRule> { euRule, euRule };
        var resource = new LimiterResource(apiResource, resourceList);

        var executor = new RateLimiterExecutor(resource, new RequestClientIdentifier());

        bool isExceeded = false;
        //one there more should raise limit
        for (int i = 0; i < 5; i++)
        {
            isExceeded = executor.IsExceeded();
            if (isExceeded)
            {
                break;
            }
        }
        // Eu limit should exceed and affect 
        Assert.That(isExceeded, Is.True);
    }

}