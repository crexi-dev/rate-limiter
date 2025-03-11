using System;
using System.Collections.Generic;
using NUnit.Framework;
using RateLimiter.RateLimitRules;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
[TestFixture]
    public class RateLimiterTests
    {
        private RateLimitingManager _rateLimitingManager;

        [SetUp]
        public void Setup()
        {
            _rateLimitingManager = new RateLimitingManager();
        }

        [Test]
        public void Test_FixedWindowRule_AllowsRequestWithinLimit()
        {
            var rule = new FixedWindowRule(maxRequests: 3, windowSize: TimeSpan.FromSeconds(10));
            _rateLimitingManager.ConfigureResourceRules("TestResource", new List<IRateLimitRule> { rule });

            var context = new RequestContext("TestClient", "TestResource");

            Assert.IsTrue(_rateLimitingManager.IsRequestAllowed(context));
            Assert.IsTrue(_rateLimitingManager.IsRequestAllowed(context));
            Assert.IsTrue(_rateLimitingManager.IsRequestAllowed(context));
        }

        [Test]
        public void Test_FixedWindowRule_RejectsRequestExceedingLimit()
        {
            var rule = new FixedWindowRule(maxRequests: 3, windowSize: TimeSpan.FromSeconds(10));
            _rateLimitingManager.ConfigureResourceRules("TestResource", new List<IRateLimitRule> { rule });

            var context = new RequestContext("TestClient", "TestResource");

            _rateLimitingManager.IsRequestAllowed(context);
            _rateLimitingManager.IsRequestAllowed(context);
            _rateLimitingManager.IsRequestAllowed(context);

            Assert.IsFalse(_rateLimitingManager.IsRequestAllowed(context)); 
        }

        [Test]
        public void Test_CoolingPeriodRule_AllowsRequestAfterCoolingPeriod()
        {
            var rule = new CoolingPeriodRule(TimeSpan.FromSeconds(5));
            _rateLimitingManager.ConfigureResourceRules("TestResource", new List<IRateLimitRule> { rule });

            var context = new RequestContext("TestClient", "TestResource");

            Assert.IsTrue(_rateLimitingManager.IsRequestAllowed(context));
            Assert.IsFalse(_rateLimitingManager.IsRequestAllowed(context));

            System.Threading.Thread.Sleep(5000);

            Assert.IsTrue(_rateLimitingManager.IsRequestAllowed(context));
        }

        [Test]
        public void Test_CombinedRules_AllowsRequestWhenBothRulesPass()
        {
            var fixedWindowRule = new FixedWindowRule(maxRequests: 3, windowSize: TimeSpan.FromSeconds(10));
            var coolingPeriodRule = new CoolingPeriodRule(TimeSpan.FromSeconds(5));
            _rateLimitingManager.ConfigureResourceRules("TestResource", [fixedWindowRule, coolingPeriodRule]);

            var context = new RequestContext("TestClient", "TestResource");

            Assert.IsTrue(_rateLimitingManager.IsRequestAllowed(context));
            Assert.IsFalse(_rateLimitingManager.IsRequestAllowed(context));

            System.Threading.Thread.Sleep(5000);

            Assert.IsTrue(_rateLimitingManager.IsRequestAllowed(context));

            _rateLimitingManager.IsRequestAllowed(context); 
            _rateLimitingManager.IsRequestAllowed(context);

            Assert.IsFalse(_rateLimitingManager.IsRequestAllowed(context));
        }
    }
}