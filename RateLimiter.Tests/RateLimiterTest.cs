using NUnit.Framework;
using RateLimiter.Rules;
using System;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [TestCase(5, 10, 50, true)]
        [TestCase(5, 20, 1, false)]
        public void Test_PerTimeSpanRule(int ruleCount, int executeCount, int waitTimeoutMilliseconds, bool expected)
        {
            var rule = new PerTimeSpanRule(ruleCount, TimeSpan.FromMilliseconds(100));
            var result = true;

            for (int i = 0; i < executeCount; i++)
            {
                result = result && rule.IsAllowed("token");
                Thread.Sleep(waitTimeoutMilliseconds);
            }

            Assert.AreEqual(result, expected);
        }

        [Test]
        public void Test_SinceLastCallRule()
        {
            var token = "token";
            var rule = new SinceLastCallRule(TimeSpan.FromMilliseconds(1000));

            var result = rule.IsAllowed(token);
            Assert.IsTrue(result);

            result = rule.IsAllowed(token);
            Assert.IsFalse(result);

            Thread.Sleep(1000);

            result = rule.IsAllowed(token);
            Assert.IsTrue(result);
        }
    }
}
