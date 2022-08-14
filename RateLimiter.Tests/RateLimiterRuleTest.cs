using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterRuleTest
    {
        RequestInTimeSpan rule1;
        TimeSpanSinceLast rule2;

        [SetUp]
        public void Init()
        {
            rule1 = new RequestInTimeSpan(Guid.NewGuid().ToString(), string.Empty, 2, TimeSpan.FromSeconds(1));
            rule2 = new TimeSpanSinceLast(Guid.NewGuid().ToString(), string.Empty, TimeSpan.FromSeconds(1));
        }

        [Test]
        public void RequestInTimeSpanInLimit()
        {
            var limit1 = rule1.IsValid();
            var limit2 = rule1.IsValid();
            bool isValid = limit1 && limit2;

            Assert.IsTrue(isValid);
        }

        [Test]
        public void RequestInTimeSpanOffLimit()
        {
            var limit = rule1.IsValid();
            var limit2 = rule1.IsValid();
            var limit3 = rule1.IsValid();
            bool isValid = limit && limit2 && limit3;

            Assert.IsFalse(isValid);
        }

        [Test]
        public void TimeSpanSinceLastInLimit()
        {
            var limit = rule2.IsValid();
            Assert.IsTrue(limit);
        }

        [Test(Description = "Doing call after timespan had passed since last")]
        public void TimeSpanSinceLastInLimitAfterWindow()
        {
            rule2.IsValid();
            Thread.Sleep(2000);
            var lease = rule2.IsValid();

            Assert.IsTrue(lease);
        }

        [Test]
        public void TimeSpanSinceLastOffLimit()
        {
            var limit = rule2.IsValid();
            var limit2 = rule2.IsValid();
            bool isValid = limit && limit2;
            Assert.IsFalse(isValid);
        }
    }
}
