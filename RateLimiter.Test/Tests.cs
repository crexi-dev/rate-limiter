using Moq;
using NUnit.Framework;
using RateLimiter.API;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void RuleA_Limit_Check_Success_Case()
        {
            var userId = 4567;

            RuleALimitAttribute ruleALimitAttribute = new();

            ruleALimitAttribute.CheckLimits(userId);
            Thread.Sleep(5000);
            ruleALimitAttribute.CheckLimits(userId);
            Thread.Sleep(5000);
            ruleALimitAttribute.CheckLimits(userId);
            Thread.Sleep(5000);

            Assert.IsTrue(ruleALimitAttribute.CheckLimits(userId));
        }

        [Test]
        public void RuleA_Limit_Check_Fail_Case()
        {
            var userId = 3456;

            RuleALimitAttribute ruleALimitAttribute = new();

            ruleALimitAttribute.CheckLimits(userId);
            Thread.Sleep(3000);
            ruleALimitAttribute.CheckLimits(userId);
            Thread.Sleep(3000);
            ruleALimitAttribute.CheckLimits(userId);
            Thread.Sleep(3000);
            ruleALimitAttribute.CheckLimits(userId);
            Thread.Sleep(3000);
            ruleALimitAttribute.CheckLimits(userId);
            Thread.Sleep(3000);

            Assert.IsFalse(ruleALimitAttribute.CheckLimits(userId));
        }

        [Test]
        public void RuleB_Limit_Check_Success_Case()
        {
            var userId = 1234;

            RuleBLimitAttribute ruleALimitAttribute = new();

            ruleALimitAttribute.CheckLimits(userId);
            Thread.Sleep(22000);
            Assert.IsTrue(ruleALimitAttribute.CheckLimits(userId));           
        }

        [Test]
        public void RuleB_Limit_Check_Fail_Case()
        {
            var userId = 2345;

            RuleBLimitAttribute ruleALimitAttribute = new();

            ruleALimitAttribute.CheckLimits(userId);
            Thread.Sleep(17000);
            Assert.IsFalse(ruleALimitAttribute.CheckLimits(userId));
        }
    }
}
