using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimitRuleTests
    {
        [Test]
        public void RequestCountPerTimespanRule_AllowsRequestsWithinLimit()
        {
            var rule = new RequestCountPerTimespanRule(5, TimeSpan.FromMinutes(1));

            for (int i = 0; i < 5; i++)
            {
                Assert.IsTrue(rule.AllowRequest("client1"));
            }

            Assert.IsFalse(rule.AllowRequest("client1"));
        }

        [Test]
        public void CooldownBetweenRequestsRule_AllowsRequestsAfterCooldown()
        {
            var rule = new CooldownBetweenRequestsRule(TimeSpan.FromSeconds(1));

            Assert.IsTrue(rule.AllowRequest("client1"));
            Assert.IsFalse(rule.AllowRequest("client1"));

            Thread.Sleep(1000);

            Assert.IsTrue(rule.AllowRequest("client1"));
        }

        [Test]
        public void GeoBasedRateLimitRule_UsesCorrectRuleBasedOnRegion()
        {
            var usRule = new RequestCountPerTimespanRule(5, TimeSpan.FromMinutes(1));
            var euRule = new CooldownBetweenRequestsRule(TimeSpan.FromSeconds(1));
            var geoRule = new GeoBasedRateLimitRule(usRule, euRule, GetClientRegion);

            for (int i = 0; i < 5; i++)
            {
                Assert.IsTrue(geoRule.AllowRequest("us_client"));
            }

            Assert.IsFalse(geoRule.AllowRequest("us_client"));

            Assert.IsTrue(geoRule.AllowRequest("eu_client"));
            Assert.IsFalse(geoRule.AllowRequest("eu_client"));
        }

        [Test]
        public void CombinedRateLimitRule_CombinesMultipleRules()
        {
            var rule1 = new RequestCountPerTimespanRule(5, TimeSpan.FromMinutes(1));
            var rule2 = new CooldownBetweenRequestsRule(TimeSpan.FromSeconds(1));
            var combinedRule = new CombinedRateLimitRule(new List<RateLimitRule> { rule1, rule2 });

            for (int i = 0; i < 5; i++)
            {
                bool result = combinedRule.AllowRequest("client1");
                Assert.IsTrue(result);
                Thread.Sleep(1000);
            }

            Assert.IsFalse(combinedRule.AllowRequest("client1"));
        }

        [Test]
        public void DailyLimitRule_AllowsRequestsWithinDailyLimit()
        {
            var rule = new DailyLimitRule(10);

            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(rule.AllowRequest("client1"));
            }

            Assert.IsFalse(rule.AllowRequest("client1"));
        }

        [Test]
        public void BurstLimitRule_AllowsRequestsWithinBurstLimit()
        {
            var rule = new BurstLimitRule(5, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(10));

            for (int i = 0; i < 5; i++)
            {
                Assert.IsTrue(rule.AllowRequest("client1"));
            }

            Assert.IsFalse(rule.AllowRequest("client1"));
        }

        [Test]
        public void TimeOfDayLimitRule_AllowsRequestsWithinTimeOfDay()
        {
            var now = DateTime.UtcNow;
            var startTime = now.TimeOfDay - TimeSpan.FromHours(1);
            var endTime = now.TimeOfDay + TimeSpan.FromHours(1);

            var rule = new TimeOfDayLimitRule(startTime, endTime);

            Assert.IsTrue(rule.AllowRequest("client1"));
        }

        [Test]
        public void UserLevelRateLimitRule_AllowsRequestsBasedOnUserLevel()
        {
            var rule = new UserLevelRateLimitRule(GetUserLevel);
            rule.ConfigureUserLevel("free", new RequestCountPerTimespanRule(5, TimeSpan.FromMinutes(1)));
            rule.ConfigureUserLevel("premium", new RequestCountPerTimespanRule(50, TimeSpan.FromMinutes(1)));

            for (int i = 0; i < 5; i++)
            {
                Assert.IsTrue(rule.AllowRequest("free_user"));
            }

            Assert.IsFalse(rule.AllowRequest("free_user"));

            for (int i = 0; i < 50; i++)
            {
                Assert.IsTrue(rule.AllowRequest("premium_user"));
            }

            Assert.IsFalse(rule.AllowRequest("premium_user"));
        }

        [Test]
        public void IPAddressRateLimitRule_AllowsRequestsBasedOnIPAddress()
        {
            var rule = new IPAddressRateLimitRule(5, TimeSpan.FromMinutes(1), GetClientIPAddress);

            for (int i = 0; i < 5; i++)
            {
                Assert.IsTrue(rule.AllowRequest("client1"));
            }

            Assert.IsFalse(rule.AllowRequest("client1"));
        }

        private string GetClientRegion(string clientId)
        {
            return clientId.StartsWith("us") ? "US" : "EU";
        }

        private string GetUserLevel(string clientId)
        {
            return clientId.StartsWith("free") ? "free" : "premium";
        }

        private string GetClientIPAddress(string clientId)
        {
            return clientId.StartsWith("client") ? "192.168.0.1" : "10.0.0.1";
        }
    }
}
