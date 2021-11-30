using System;
using System.Threading.Tasks;
using NUnit.Framework;
using RateLimiter.Limits;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class ThrottlingLimitTest
    {
        [Test]
        public void CanInvoke_NoInvokesBefore_ReturnTrue()
        {
            var limit = new ThrottlingLimit { Parameters = new ThrottlingLimit.ThrottlingLimitParameters(5, TimeSpan.FromSeconds(10)) };

            Assert.IsTrue(limit.CanInvoke());
        }

        [Test]
        public void CanInvoke_MoreFrequently_ReturnFalse()
        {
            var limit = new ThrottlingLimit { Parameters = new ThrottlingLimit.ThrottlingLimitParameters(2, TimeSpan.FromSeconds(10)) };

            limit.CanInvoke();
            limit.CanInvoke();

            Assert.IsFalse(limit.CanInvoke());
        }

        [Test]
        public void CanInvoke_LessFrequently_ReturnTrue()
        {
            var limit = new ThrottlingLimit { Parameters = new ThrottlingLimit.ThrottlingLimitParameters(2, TimeSpan.FromSeconds(2)) };

            limit.CanInvoke();

            Task.Delay(1100).Wait();

            limit.CanInvoke();

            Task.Delay(1100).Wait();

            Assert.IsTrue(limit.CanInvoke());
        }

        [Test]
        public void CanInvoke_LessFrequentlyAfterNotAllowedCall_ReturnTrue()
        {
            var limit = new ThrottlingLimit { Parameters = new ThrottlingLimit.ThrottlingLimitParameters(2, TimeSpan.FromSeconds(2)) };

            limit.CanInvoke();
            limit.CanInvoke();
            limit.CanInvoke();

            Task.Delay(2100).Wait();

            Assert.IsTrue(limit.CanInvoke());
        }
    }
}
