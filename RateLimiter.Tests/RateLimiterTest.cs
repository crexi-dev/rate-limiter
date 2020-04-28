using NUnit.Framework;
using RateLimiter;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        API api;
       
        RateLimiter rateLimiter = RateLimiter.Instance;
        string getCallExpectedOutput = "Success";
        string getCallActualOutput = null;

        public RateLimiterTest()
        {
            api = new API();
        }

        [Test]
        public void SimpleRuleWithLeakyBucketStrategyNoFilters()
        {
            User user = api.AuthenticateUser("test auth token");

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
            User user = api.AuthenticateUser("test auth token");

            int fixedWindowMaxRequestsPerSescond = 2;
            int fixedWindowTimeWindowInSeconds = 5;
            var filter = new LocationBasedFilter { CountryCode = user.CountryCode };
            RateLimiterRule rule = new RateLimiterRule(new FixedWindowStrategy(fixedWindowMaxRequestsPerSescond, fixedWindowTimeWindowInSeconds), new LocationBasedFilter { CountryCode = "US" });

            int expectedAllowedCount = 4, actualAllowedCount = 0;
          
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
        public void SimpleRuleWithFixedWindowStrategyAndCountryFilter1()
        {
            User user = api.AuthenticateUser("test auth token");

            int fixedWindowMaxRequestsPerWindow = 2;
            int fixedWindowTimeWindowInSeconds = 5;
            var filter = new LocationBasedFilter { CountryCode = user.CountryCode };
            RateLimiterRule rule = new RateLimiterRule(new FixedWindowStrategy(fixedWindowMaxRequestsPerWindow, fixedWindowTimeWindowInSeconds), new LocationBasedFilter { CountryCode = "US" });
            
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

        }


        [Test]
        public void MultipleRulesUSUser()
        {
            User user = api.AuthenticateUser("test auth token");
            var filter = new LocationBasedFilter { CountryCode = user.CountryCode };
            
            //leacky bucket params
            int maximumRequestQuota = 2;
            int restoreRateAmount = 1;
            int restoreRateTimeAmount = 2;
            EnumRestoreRateTimePeriod restoreRateTimePeriod = EnumRestoreRateTimePeriod.Seconds;
            
            //fixed findow params
            int fixedWindowStrategyMaxRequestsPerWindow = 2;
            int fixedWindowStrategyTimeWindowInSeconds = 2;

            List<RateLimiterRule> rules = new List<RateLimiterRule> {
                new RateLimiterRule(new FixedWindowStrategy(fixedWindowStrategyMaxRequestsPerWindow,fixedWindowStrategyTimeWindowInSeconds), 
                                     new LocationBasedFilter { CountryCode = "US" }),
                new RateLimiterRule(new LeakyBucketStrategy(maximumRequestQuota, restoreRateAmount, restoreRateTimeAmount, restoreRateTimePeriod), 
                                    new LocationBasedFilter { CountryCode="EU" })
            };

            int expectedAllowedCount = 8, actualAllowedCount = 0;

            //trigger Rate limiting with different users with US token. FixedWindow expected. Requests 1,2,4,5 allowed, rest are denied
            for (int i = 1; i <= 20; i++)
            {
                if (rateLimiter.ValidatedRuleList(user.Id, i, rules, filter))
                {
                    actualAllowedCount++;
                    getCallActualOutput = api.DoGetCall(user);
                    TestContext.WriteLine("Allowed request id: {0}", i);
                }
                else
                    TestContext.WriteLine("Denied request id: {0}", i);
                if (i % 5 == 0)
                    Thread.Sleep(2500);
            }
            TestContext.WriteLine("actualAllowCount: {0}", actualAllowedCount);

            Assert.AreEqual(expectedAllowedCount, actualAllowedCount);

           
        }
        [Test]
        public void MultipleRulesEUUser()
        {
            User user = api.AuthenticateUser("test auth token");
            user.CountryCode = "EU";
            var filter = new LocationBasedFilter { CountryCode = user.CountryCode };
            //leacky bucket params
            int maximumRequestQuota = 2;
            int restoreRateAmount = 1;
            int restoreRateTimeAmount = 2;
            EnumRestoreRateTimePeriod restoreRateTimePeriod = EnumRestoreRateTimePeriod.Seconds;

            int fixedWindowStrategyMaxRequestsPerWindow = 10;
            int fixedWindowStrategyTimeWindowInSeconds = 2;

            List<RateLimiterRule> rules = new List<RateLimiterRule> {
                new RateLimiterRule(new FixedWindowStrategy(fixedWindowStrategyMaxRequestsPerWindow,fixedWindowStrategyTimeWindowInSeconds),
                                     new LocationBasedFilter { CountryCode = "US" }),
                new RateLimiterRule(new LeakyBucketStrategy(maximumRequestQuota, restoreRateAmount, restoreRateTimeAmount, restoreRateTimePeriod),
                                    new LocationBasedFilter { CountryCode="EU" })
            };

            int expectedAllowedCount = 5;
            int actualAllowedCount = 0;
           
            user.Id = 200;
            //  trigger Rate limiting with different users with EU token.LeakyBucket expected. Requests 1,2,4,5 allowed, rest are denied
            for (int i = 1; i <= 20; i++)
            {
                if (rateLimiter.ValidatedRuleList(user.Id, i, rules, filter))
                {
                    actualAllowedCount++;
                    getCallActualOutput = api.DoGetCall(user);
                    TestContext.WriteLine("Allowed request id: {0}", i);
                }
                else
                    TestContext.WriteLine("Denied request id: {0}", i);
                if (i % 5 == 0)
                    Thread.Sleep(2500);
            }
            TestContext.WriteLine("actualAllowCount: {0}", actualAllowedCount);
            
            Assert.AreEqual(expectedAllowedCount, actualAllowedCount);
        }
    }
}
