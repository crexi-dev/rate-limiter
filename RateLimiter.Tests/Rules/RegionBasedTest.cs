using System;
using System.Collections.Generic;
using NUnit.Framework;
using RateLimiter.Core.Rules;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RegionBasedRuleTest
    {
        private string GetRegion(string token)
        {
            if (token.StartsWith("EU"))
                return "EU";
            return "US";
        }

        [Test]
        public void TestApplyCorrectRuleForRegion()
        {
            // US rule allows 2 requests, EU rule allows 1 request
            var usRule = new FixedWindowRule(2, TimeSpan.FromSeconds(10));
            var euRule = new FixedWindowRule(1, TimeSpan.FromSeconds(10));

            var regionRules = new Dictionary<string, IRateLimitRule>
            {
                { "US", usRule },
                { "EU", euRule }
            };

            var rule = new RegionBasedRule(regionRules, GetRegion);

            // US client should get 2 requests
            Assert.That(rule.IsAllowed("US-client", "api/test"), Is.True, "First US request should be allowed");
            Assert.That(rule.IsAllowed("US-client", "api/test"), Is.True, "Second US request should be allowed");
            Assert.That(rule.IsAllowed("US-client", "api/test"), Is.False, "Third US request should be denied");

            // EU client should get 1 request
            Assert.That(rule.IsAllowed("EU-client", "api/test"), Is.True, "First EU request should be allowed");
            Assert.That(rule.IsAllowed("EU-client", "api/test"), Is.False, "Second EU request should be denied");
        }
    }
}