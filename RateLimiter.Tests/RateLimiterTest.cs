using NUnit.Framework;
using RateLimiter;

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

            int requestQuota = 0;
            int maximumRequestQuota = 10;
            int restoreRateAmount = 1;
            int restoreRateTimeAmount = 2;
            EnumRestoreRateTimePeriod restoreRateTimePeriod = EnumRestoreRateTimePeriod.Seconds;

            RateLimiterRule rule = new RateLimiterRule(new LeakyBucketStrategy(requestQuota,maximumRequestQuota,restoreRateAmount,restoreRateTimeAmount,restoreRateTimePeriod));

            int requestId = 123;

            if (rateLimiter.ValidateRule(user.Id, requestId, rule))
                getCallActualOutput = api.DoGetCall(user);
         
            Assert.AreEqual(getCallExpectedOutput,getCallActualOutput);
        }
    }
}
