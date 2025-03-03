using NUnit.Framework;
using RateLimiter.Rules.FixedWindowRule;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests.Rules;

[TestFixture]
public class FixedWindowRuleTest: TestBase
{
    [Test]
    public void FixedWindowRateLimit_AllowsRequestsWithinLimit_ValidConditions()
    {
        var options = new FixedWindowRuleOptions
        {
            RuleConditions = new List<Func<AccessToken, bool>>
            {
                token => token.Region == Region.eu,
                token => !string.IsNullOrEmpty(token.UserId)
            },
            WindowSize = TimeSpan.FromMinutes(1),
            Limit = 5
        };

        var rule = new FixedWindowRule(options, _cache);
        var token = new AccessToken("test_client", Region.eu);
        var route = "/test";

        for (int i = 0; i < options.Limit; i++)
        {
            Assert.IsTrue(rule.IsRequestAllowed(token, route));
        }
    }

    [Test]
    public void FixedWindowRateLimit_AllowsRequestsWithinLimit_NoConditions()
    {
        var options = new FixedWindowRuleOptions
        {
            WindowSize = TimeSpan.FromMinutes(1),
            Limit = 5
        };

        var rule = new FixedWindowRule(options, _cache);
        var token = new AccessToken("test_client", Region.eu);
        var route = "/test";

        for (int i = 0; i < options.Limit; i++)
        {
            Assert.IsTrue(rule.IsRequestAllowed(token, route));
        }
    }

    [Test]
    public void FixedWindowRateLimit_BlocksRequestsBeyondLimit()
    {
        var options = new FixedWindowRuleOptions
        {
            RuleConditions = new List<Func<AccessToken, bool>>
            {
                token => token.Region == Region.eu,
            },
            WindowSize = TimeSpan.FromMinutes(1),
            Limit = 3

        };

        var rule = new FixedWindowRule(options, _cache);
        var token = new AccessToken("test_client", Region.eu);
        var route = "/test";

        for (int i = 0; i < options.Limit; i++)
        {
            Assert.IsTrue(rule.IsRequestAllowed(token, route));
        }

        Assert.IsFalse(rule.IsRequestAllowed(token, route));
    }


    [Test]
    public void FixedWindowRateLimit_DoNotApplyRule()
    {
        var options = new FixedWindowRuleOptions
        {
            RuleConditions = new List<Func<AccessToken, bool>>
            {
                token => token.Region == Region.eu,
            },
            WindowSize = TimeSpan.FromMinutes(1),
            Limit = 3

        };

        var rule = new FixedWindowRule(options, _cache);
        var token = new AccessToken("test_client", Region.us);
        var route = "/test";

        for (int i = 0; i <= options.Limit + 5; i++)
        {
            Assert.IsTrue(rule.IsRequestAllowed(token, route));
        }
    }
}
