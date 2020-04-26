using NUnit.Framework;
using RateLimiter;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        API api;
        User user;
        RateLimiter rateLimiter = RateLimiter.Instance;
        string getCallExpectedOutput = "Sucess";
        string getCallActualOutput = null;

        public RateLimiterTest()
        {
            api = new API();
        }

        [Test]
        public void SimpleRuleWithLeakyBucketStrategyNoFilters()
        {
            user = api.AuthenticateUser("test auth token");

            int maximumRequestQuota = 10;
            int restoreRateAmount = 1;
            int restoreRateTimeAmount = 2;
            EnumRestoreRateTimePeriod restoreRateTimePeriod = EnumRestoreRateTimePeriod.Seconds;

            int requestId = 123;
            RateLimiterRule rule = new RateLimiterRule(new LeakyBucketStrategy(maximumRequestQuota, restoreRateAmount, restoreRateTimeAmount, restoreRateTimePeriod));
            if (rateLimiter.ValidateRule(user.Id, requestId, rule ))
                getCallActualOutput = api.DoGetCall(user);
         
            Assert.AreEqual(getCallExpectedOutput,getCallActualOutput);
        }

        [Test]
        public void SimpleRuleWithFixedWindowStrategyAndCountryFilter()
        {
            user = api.AuthenticateUser("test auth token");

            int fixedWindowMaxRequestsPerSescond = 2;
            int fixedWindowTimeWindowInSeconds = 10;

            int requestId = 123;
            RateLimiterRule rule = new RateLimiterRule(new FixedWindowStrategy(fixedWindowMaxRequestsPerSescond, fixedWindowTimeWindowInSeconds), new LocationBasedFilter { CountryCode = "US" });
            if (rateLimiter.ValidateRule(user.Id, requestId, rule, new LocationBasedFilter { CountryCode = user.CountryCode })) 
                getCallActualOutput = api.DoGetCall(user);

            Assert.AreEqual(getCallExpectedOutput, getCallActualOutput);
        }


        [Test]
        public void MultipleRules()
        {
            user = api.AuthenticateUser("test auth token");

            int maximumRequestQuota = 10;
            int restoreRateAmount = 1;
            int restoreRateTimeAmount = 2;
            EnumRestoreRateTimePeriod restoreRateTimePeriod = EnumRestoreRateTimePeriod.Seconds;

            int fixedWindowMaxRequestsPerSescond = 2;
            int fixedWindowTimeWindowInSeconds = 10;
            int requestId = 123;

            List<RateLimiterRule> rules = new List<RateLimiterRule> { 
                new RateLimiterRule(new FixedWindowStrategy(fixedWindowMaxRequestsPerSescond,fixedWindowTimeWindowInSeconds), new LocationBasedFilter { CountryCode = "US" }),
                new RateLimiterRule(new LeakyBucketStrategy(maximumRequestQuota, restoreRateAmount, restoreRateTimeAmount, restoreRateTimePeriod))
            };
            if (rateLimiter.ValidatedRuleList(user.Id, requestId, rules, new LocationBasedFilter { CountryCode = user.CountryCode }))
                getCallActualOutput = api.DoGetCall(user);

            Assert.AreEqual(getCallExpectedOutput, getCallActualOutput);
        }
    }
}
