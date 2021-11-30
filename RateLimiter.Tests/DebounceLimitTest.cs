using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RateLimiter.Limits;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class DebounceLimitTest
    {
        [Test]
        public void CanInvoke_NoInvokesBefore_ReturnTrue()
        {
            var limit = new DebounceLimit { Parameters = new DebounceLimit.DebounceLimitParameters(TimeSpan.FromSeconds(10)) };

            Assert.IsTrue(limit.CanInvoke());
        }

        [Test]
        public void CanInvoke_MoreFrequently_ReturnFalse()
        {
            var limit = new DebounceLimit { Parameters = new DebounceLimit.DebounceLimitParameters(TimeSpan.FromSeconds(10)) };
            limit.CanInvoke();

            Assert.IsFalse(limit.CanInvoke());
        }

        [Test]
        public void CanInvoke_LessFrequently_ReturnTrue()
        {
            var limit = new DebounceLimit { Parameters = new DebounceLimit.DebounceLimitParameters(TimeSpan.FromSeconds(1)) };
            limit.CanInvoke();
            Task.Delay(2000).Wait();

            Assert.IsTrue(limit.CanInvoke());
        }
    }
}
