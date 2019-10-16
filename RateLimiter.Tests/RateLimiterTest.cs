using NUnit.Framework;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public async Task ShouldEnforceRequestsPerTimeSpanRule()
        {
            var clientId = Guid.NewGuid();
            RateLimitService rateService = new RateLimitService();
            rateService.AddClientRule(Max3CallsPerTenSecond(clientId));
            SampleApi api = new SampleApi(rateService);

            var response1 = api.SampleApiMethod(clientId);
            Assert.IsFalse(response1.RateLimitError);
            var response2 = api.SampleApiMethod(clientId);
            Assert.IsFalse(response2.RateLimitError);
            var response3 = api.SampleApiMethod(clientId);
            Assert.IsFalse(response3.RateLimitError);
            var response4 = api.SampleApiMethod(clientId);
            Assert.IsTrue(response4.RateLimitError); //limit reached
            await Task.Delay(10000);
            var response5 = api.SampleApiMethod(clientId);
            Assert.IsFalse(response5.RateLimitError);
            var response6 = api.SampleApiMethod(clientId);
            Assert.IsFalse(response6.RateLimitError);
            var response7 = api.SampleApiMethod(clientId);
            Assert.IsFalse(response7.RateLimitError);
            var response8 = api.SampleApiMethod(clientId);
            Assert.IsTrue(response8.RateLimitError); //limit reached
        }

        [Test]
        public async Task ShouldEnforceTimeSinceLastCallRule()
        {
            var clientId = Guid.NewGuid();

            RateLimitService rateService = new RateLimitService();
            rateService.AddClientRule(MustWaitFiveSecondsAfterLastCall(clientId));

            SampleApi api = new SampleApi(rateService);

            var response1 = api.SampleApiMethod(clientId);
            Assert.IsFalse(response1.RateLimitError);
            var response2 = api.SampleApiMethod(clientId);
            Assert.IsTrue(response2.RateLimitError); //limit reached   
            await Task.Delay(5000); 
            var response3 = api.SampleApiMethod(clientId);
            Assert.IsFalse(response3.RateLimitError);
            await Task.Delay(1000); 
            var response4 = api.SampleApiMethod(clientId);
            Assert.IsTrue(response4.RateLimitError); //limit reached
        }


        private RateLimitRule Max3CallsPerTenSecond(Guid clientId)
        {
            RequestsPerTimeSpanRule rule = new RequestsPerTimeSpanRule();
            rule.MaxCallsAllowed = 3;
            rule.Period = new TimeSpan(0, 0, 10);

            RateLimitRule rateRule = new RateLimitRule();
            rateRule.ClientId = clientId;
            rateRule.RequestsPerTimeSpanRule = rule;

            return rateRule;
        }

        private RateLimitRule MustWaitFiveSecondsAfterLastCall(Guid clientId)
        {
            TimeSinceLastCallRule rule = new TimeSinceLastCallRule();
            rule.Period = new TimeSpan(0, 0, 5);

            RateLimitRule rateRule = new RateLimitRule();
            rateRule.ClientId = clientId;
            rateRule.TimeSinceLastCallRule = rule;

            return rateRule;
        }
    }
}
