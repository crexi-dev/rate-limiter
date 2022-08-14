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
            var limit = rule1.GetLimiter();
            var lease = limit.Acquire();
            var lease2 = limit.Acquire();
            bool isValid = lease.IsAcquired && lease2.IsAcquired;

            Assert.IsTrue(isValid);
        }

        [Test]
        public void RequestInTimeSpanOffLimit()
        {
            var limit = rule1.GetLimiter();
            var lease = limit.Acquire();
            var lease2 = limit.Acquire();
            var lease3 = limit.Acquire();
            bool isValid = lease.IsAcquired && lease2.IsAcquired && lease3.IsAcquired;

            Assert.IsFalse(isValid);
        }

        [Test]
        public void TimeSpanSinceLastInLimit()
        {
            var limit = rule2.GetLimiter();
            var lease = limit.Acquire();
            Assert.IsTrue(lease.IsAcquired);
        }

        [Test(Description = "Doing call after timespan had passed since last")]
        public void TimeSpanSinceLastInLimitAfterWindow()
        {
            var limit = rule2.GetLimiter();
            limit.Acquire(1);
            Thread.Sleep(2000);
            var lease = limit.Acquire();

            Assert.IsTrue(lease.IsAcquired);
        }

        [Test]
        public void TimeSpanSinceLastOffLimit()
        {
            var limit = rule2.GetLimiter();
            var lease = limit.Acquire();
            var lease2 = limit.Acquire();
            bool isValid = lease.IsAcquired && lease2.IsAcquired;
            Assert.IsFalse(isValid);
        }
    }
}
