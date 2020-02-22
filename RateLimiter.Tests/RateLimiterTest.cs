using NUnit.Framework;
using RateLimiter.Rules;
using RateLimiter.State;
using System;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private IRulePersist _ruleState;

        [OneTimeSetUp]
        public void Startup()
        {
            _ruleState = InMemoryRuleState.GetInstance;
        }

        [Test]
        public void X_Request_Per_Timespan_First_Request()
        {
            var access_token = Guid.NewGuid().ToString();
            var req = new RequestInfo { Access_Token = access_token };
            var rule = new RuleEval();
            rule.Eval(new RequestPerElapsedTime(_ruleState, TimeSpan.FromSeconds(10), 3));

            var rslt = rule.Evaluate(req);

            Assert.IsFalse(rslt);
        }

        [Test]
        public void X_Request_Per_Timespan_GreaterThanCnt_1()
        {
            var access_token = Guid.NewGuid().ToString();
            var req = new RequestInfo { Access_Token = access_token };
            var rule = new RuleEval();
            rule.Eval(new RequestPerElapsedTime(_ruleState, TimeSpan.FromSeconds(10), 1));

            var rslt = rule.Evaluate(req);
            Assert.IsFalse(rslt);

            rslt = rule.Evaluate(req);

            Assert.IsTrue(rslt);
        }

        [Test]
        public void Combine_Rule_Country_US_And_X_Request_Per_Timespan_GreaterThanCnt_()
        {
            var access_token = Guid.NewGuid().ToString();
            var req = new RequestInfo { Access_Token = access_token, Country="US" };
            var rule = new RuleEval();
            rule.Eval(new RequestFromCountry("US"));
            rule.AndRule(new RequestPerElapsedTime(_ruleState, TimeSpan.FromSeconds(10), 1));

            var rslt = rule.Evaluate(req);
            Assert.IsFalse(rslt);

            rslt = rule.Evaluate(req);

            Assert.IsTrue(rslt);
        }

        [Test]
        public void Combine_Rule_Country_US_And_X_Request_Per_Timespan_GreaterThanCnt_1_UK_Request()
        {
            var access_token = Guid.NewGuid().ToString();
            var req = new RequestInfo { Access_Token = access_token, Country = "UK" };
            var rule = new RuleEval();
            rule.Eval(new RequestFromCountry("US"));
            rule.AndRule(new RequestPerElapsedTime(_ruleState, TimeSpan.FromSeconds(10), 1));

            var rslt = rule.Evaluate(req);
            Assert.IsFalse(rslt);

            rslt = rule.Evaluate(req);

            Assert.IsFalse(rslt);
        }

        [Test]
        public void Complex_Rule_Combination_And_Or()
        {
            var access_token = Guid.NewGuid().ToString();

            var req = new RequestInfo { Access_Token = access_token, Country = "EU" };
            var rule = new RuleEval();

            var usaRule = ChainHelper.AndChain(new RequestFromCountry("US"), new RequestPerElapsedTime(_ruleState, TimeSpan.FromSeconds(10), 1));
            var euRule = ChainHelper.AndChain(new RequestFromCountry("EU"), new RequestElapsedTime(_ruleState, TimeSpan.FromSeconds(5)));
            
            rule.AndChain(usaRule);
            rule.OrChain(euRule);

            var rslt = rule.Evaluate(req);
            Assert.IsFalse(rslt);

            //Should match a rule EU and 2nd request inside elapsed time

            rslt = rule.Evaluate(req);
            Assert.IsTrue(rslt);


            // Request from US
            var us_access_token = Guid.NewGuid().ToString();

            var us_req = new RequestInfo { Access_Token = us_access_token, Country = "US" };
            rslt = rule.Evaluate(us_req);
            Assert.IsFalse(rslt);

            // 2nd request
            rslt = rule.Evaluate(us_req);
            Assert.IsTrue(rslt);

        }
    }
}