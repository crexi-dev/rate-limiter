using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RateLimiter.Rules;
using RateLimiter.Tests.Rules_Integration;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class RateLimitRuleTests
    {
        [TestCase("test")]
        [TestCase("test2")]
        public void RateLimitRule_LimitPeriod_Success(string token)
        {
            var model = new Models.RateLimitRuleModel { Endpoint = "api/test", RequestLimit = 2, RequestPeriod = 1 };
            var limitperiod = new LimitPeriodRule(SingletonProvider.ServiceProvider.GetService<IMemoryCache>());


            Assert.IsTrue(limitperiod.IsValid(model, token));
            Assert.IsTrue(limitperiod.IsValid(model, token));
            Assert.IsFalse(limitperiod.IsValid(model, token));
        }

        [TestCase("test")]
        [TestCase("test2")]
        public void RateLimitRule_Request_Interval_Success(string token)
        {
            var model = new Models.RateLimitRuleModel { Endpoint = "api/test", RequestPeriod = 1 };
            var intervalRule = new RequestIntervalRule(SingletonProvider.ServiceProvider.GetService<IMemoryCache>());


            Assert.IsTrue(intervalRule.IsValid(model, token));
            Assert.IsFalse(intervalRule.IsValid(model, token));
        }
    }
}
