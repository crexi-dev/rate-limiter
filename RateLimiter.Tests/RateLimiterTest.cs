using NUnit.Framework;
using RateLimiter;
using System;
using System.Collections.Generic;
using System.Threading;

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
            int fixedWindowTimeWindowInSeconds = 5;
            var filter = new LocationBasedFilter { CountryCode = user.CountryCode };
            RateLimiterRule rule = new RateLimiterRule(new FixedWindowStrategy(fixedWindowMaxRequestsPerSescond, fixedWindowTimeWindowInSeconds), new LocationBasedFilter { CountryCode = "US" });
            //create loop to call many requests to trigger DOS
            
            int expectedAllowedCount = 2, actualAllowedCount = 0;
            for (int i = 1; i <= 10; i++)
            {
                if (rateLimiter.ValidateRule(user.Id, i, rule, filter))
                {
                    getCallActualOutput = api.DoGetCall(user);
                    TestContext.WriteLine("Allowed request id: {0}", i);
                    actualAllowedCount++;
                }
                else
                    TestContext.WriteLine("Denied request id: {0}", i);
            }

            Assert.AreEqual(expectedAllowedCount, actualAllowedCount);

            expectedAllowedCount = 8;
            actualAllowedCount = 0;
            for (int i = 1; i <= 10; i++)
            {
                if (rateLimiter.ValidateRule(user.Id, i, rule, filter))
                {
                    getCallActualOutput = api.DoGetCall(user);
                    TestContext.WriteLine("Allowed request id: {0}", i);
                    actualAllowedCount++;
                }
                else
                    TestContext.WriteLine("Denied request id: {0}", i);

                if (i == 2)
                    Thread.Sleep(5000);
            }

            Assert.AreEqual(expectedAllowedCount, actualAllowedCount);
        }


        [Test]
        public void MultipleRules()
        {
            user = api.AuthenticateUser("test auth token");
            var filter = new LocationBasedFilter { CountryCode = user.CountryCode };
            int maximumRequestQuota = 10;
            int restoreRateAmount = 1;
            int restoreRateTimeAmount = 2;
            EnumRestoreRateTimePeriod restoreRateTimePeriod = EnumRestoreRateTimePeriod.Seconds;

            int fixedWindowMaxRequestsPerSescond = 2;
            int fixedWindowTimeWindowInSeconds = 10;

            List<RateLimiterRule> rules = new List<RateLimiterRule> {
                new RateLimiterRule(new FixedWindowStrategy(fixedWindowMaxRequestsPerSescond,fixedWindowTimeWindowInSeconds), new LocationBasedFilter { CountryCode = "US" }),
                new RateLimiterRule(new LeakyBucketStrategy(maximumRequestQuota, restoreRateAmount, restoreRateTimeAmount, restoreRateTimePeriod), new LocationBasedFilter { CountryCode="EU" })
            };


            //trigger DOS with different users with US and EU token
            int requestId = 123;

          
            if (rateLimiter.ValidatedRuleList(user.Id, requestId, rules, filter))
                getCallActualOutput = api.DoGetCall(user);

            Assert.AreEqual(getCallExpectedOutput, getCallActualOutput);
        }
    }
}
