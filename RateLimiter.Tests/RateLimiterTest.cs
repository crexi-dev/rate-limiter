using NUnit.Framework;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {

        [Test]
        public void MaxRequestRateAndIdleTimePositiveFlow()
        {
            var limiter = new RateLimiter();
            var result = true;

            for (var i = 1; i <= 10; i++)
            { 
                result = limiter.EvaluateClientRequest("bec85096-6ce3-4db4-9fb7-3765dc0f4859", "/api/customer", "GET");
                if (!result) break;

                Thread.Sleep(300);
            }       
            
            Assert.IsTrue(result);
        }

        [Test]
        public void MaxRequestRateAndIdleTimeNegativeFlow()
        {
            var limiter = new RateLimiter();
            var result = true;

            for (var i = 1; i <= 10; i++)
            {
                result = limiter.EvaluateClientRequest("bec85096-6ce3-4db4-9fb7-3765dc0f4859", "/api/customer", "GET");
                if (!result) break;

                Thread.Sleep(100);
            }

            Assert.IsFalse(result);
        }

        [Test]
        public void TokenInRegionPositiveFlow()
        {
            var limiter = new RateLimiter();
            var result = true;

            for (var i = 1; i <= 10; i++)
            {
                result = limiter.EvaluateClientRequest("USbec85096-6ce3-4db4-9fb7-3765dc0f4859", "/api/rates", "GET");
                if (!result) break;

                Thread.Sleep(350);
            }

            Assert.IsTrue(result);
        }

        [Test]
        public void TokenInRegionNegativeFlow()
        {
            var limiter = new RateLimiter();
            var result = true;

            for (var i = 1; i <= 10; i++)
            {
                result = limiter.EvaluateClientRequest("USbec85096-6ce3-4db4-9fb7-3765dc0f4859", "/api/rates", "GET");
                if (!result) break;

                Thread.Sleep(150);
            }

            Assert.IsFalse(result);
        }
    }
}
