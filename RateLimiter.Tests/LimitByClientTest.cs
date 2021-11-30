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
    public class LimitByClientTest
    {
        [Test]
        public void CanInvoke_AnotherClientWasInvokedMomentAgo_ReturnTrue()
        {
            var limit = new LimitByClient<DebounceLimit>(new DebounceLimit.DebounceLimitParameters(TimeSpan.FromSeconds(10)));

            limit.CanInvoke("client A");
            Assert.IsTrue(limit.CanInvoke("client B"));
        }

        [Test]
        public void CanInvoke_SameClientWasInvokedMomentAgo_ReturnFalse()
        {
            var limit = new LimitByClient<DebounceLimit>(new DebounceLimit.DebounceLimitParameters(TimeSpan.FromSeconds(10)));

            limit.CanInvoke("client A");
            Assert.IsFalse(limit.CanInvoke("client A"));
        }
    }
}
