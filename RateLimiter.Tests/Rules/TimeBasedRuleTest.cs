using NUnit.Framework;
using RateLimiter.Rules.TimeBasedRule;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RateLimiter.Tests.Rules;

[TestFixture]
public class TimeBasedRuleTest: TestBase
{

    [Test]
    public void TimeBasedRateLimit_AllowsRequestsAfterTimeInterval()
    {
        var options = new TimeBasedRuleOptions
        {
            MinTimeBetweenRequests = TimeSpan.FromSeconds(1)
        };

        var rule = new TimeBasedRule(options, _cache);

        var token = new AccessToken("test_client", Region.eu);
        var route = "/test";

        Assert.IsTrue(rule.IsRequestAllowed(token, route));
        Assert.IsFalse(rule.IsRequestAllowed(token, route));

        Thread.Sleep(1100);
        Assert.IsTrue(rule.IsRequestAllowed(token, route));
    }

    [Test]
    public void TimeBasedRateLimit_DoNotApplyRuleForCurrentCall()
    {
        var options = new TimeBasedRuleOptions
        {
            RuleConditions = new List<Func<AccessToken, bool>>
            {
                token => token.Region == Region.us,
            },
            MinTimeBetweenRequests = TimeSpan.FromSeconds(1)
        };

        var rule = new TimeBasedRule(options, _cache);

        var token = new AccessToken("test_client", Region.eu);
        var route = "/test";

        for (int i = 0; i <= 10; i++)
        {
            Assert.IsTrue(rule.IsRequestAllowed(token, route));
        }
    }
}
