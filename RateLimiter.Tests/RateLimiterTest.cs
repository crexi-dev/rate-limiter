using NUnit.Framework;
using RateLimiter.API;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public void Make_a_call()
        {
            LimiterApi limiterApi = new LimiterApi();

            for (int i = 0; i < 1000; i++)
            {
                limiterApi.Call(new Models.RequestModel() { ClientLocation = Models.Enums.ELocation.US, UserToken = "US1" });
            }

            for (int i = 0; i < 800; i++)
            {
                limiterApi.Call(new Models.RequestModel() { ClientLocation = Models.Enums.ELocation.EU, UserToken = "EU1" });
            }

            Assert.IsTrue(true);
        }
    }
}
