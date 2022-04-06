using NUnit.Framework;
using RateLimiter.LimitRules;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class ValidationTest
    {
        public ValidationTest()
        {

        }

        [Test]
        public void Validate_ReqPerTimeLimitRule_WithinLimit()
        {
            var reqLimitRule = new ReqPerTimeLimitRule(TimeSpan.FromSeconds(5), 5);

            var resource = "/api/get";
            var identifer = "user_token";

            var validation = new Validation(new List<ILimitRule> { reqLimitRule });


            for (int i = 0; i < 4; i++)
            {
                validation.Validate(reqLimitRule.Name, resource, identifer);
                Thread.Sleep(100);
            }

            Assert.IsTrue(validation.Validate(reqLimitRule.Name, resource, identifer));
        }

        [Test]
        public void Validate_ReqPerTimeLimitRule_NotWithinLimit()
        {
            var reqLimitRule = new ReqPerTimeLimitRule(TimeSpan.FromSeconds(5), 5);

            var resource = "/api/get";
            var identifer = "user_token";

            var validation = new Validation(new List<ILimitRule> { reqLimitRule });


            for (int i = 0; i < 5; i++)
            {
                validation.Validate(reqLimitRule.Name, resource, identifer);
                Thread.Sleep(100);
            }

            Assert.IsFalse(validation.Validate(reqLimitRule.Name, resource, identifer));
        }


        [Test]
        public void Validate_ReqFromLastLimitRule_WithinLimit()
        {
            var reqLimitRule = new ReqFromLastLimitRule(TimeSpan.FromSeconds(1));

            var resource = "/api/get";
            var identifer = "user_token";

            var validation = new Validation(new List<ILimitRule> { reqLimitRule });


            validation.Validate(reqLimitRule.Name, resource, identifer);
            Thread.Sleep(1200);

            Assert.IsTrue(validation.Validate(reqLimitRule.Name, resource, identifer));
        }

        [Test]
        public void Validate_ReqFromLastLimitRule_NotWithinLimit()
        {
            var reqLimitRule = new ReqFromLastLimitRule(TimeSpan.FromSeconds(1));

            var resource = "/api/get";
            var identifer = "user_token";

            var validation = new Validation(new List<ILimitRule> { reqLimitRule });


            validation.Validate(reqLimitRule.Name, resource, identifer);
            Thread.Sleep(100);

            Assert.IsFalse(validation.Validate(reqLimitRule.Name, resource, identifer));
        }
    }
}
