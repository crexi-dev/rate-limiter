using System;
using NUnit.Framework;
// using RateLimiter;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private IOptions<Settings>? _iOptionsSettings;
        private RateLimitLibrary? _RateLimit;

        [OneTimeSetUp]
        public void Setup()
        {
            var testSettings = new Settings() { 
                Rules = new List<RuleDefinition>() { new RuleDefinition() { name = "testRule", timeSpanSeconds = 10, allowedRequests = 4 }},
                ApiRuleMapping = new Dictionary<string, List<string>>() { {"testApi", new List<string>() {"testRule"} } }
            };

            _iOptionsSettings = Options.Create<Settings>(testSettings);
            _RateLimit = new RateLimitLibrary(_iOptionsSettings);
            
            DataSimulator.UserLogData = new Dictionary<string, List<DateTime>>() { 
                {"testUserInValid:testApi", new List<DateTime>() { DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now } },
                {"testUserValid:testApi", new List<DateTime>() {  } }
            };
        }

        [Test]
        public void TestValidRateLimit()
        {
            Assert.True(_RateLimit?.IsRateLimitAccepted("testUserValid", "testApi"));
        }

        [Test]
        public void TestInValidRateLimit()
        {
            Assert.False(_RateLimit?.IsRateLimitAccepted("testUserInValid", "testApi"));
        }
    }
}
