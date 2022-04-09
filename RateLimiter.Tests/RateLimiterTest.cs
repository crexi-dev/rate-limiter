using NUnit.Framework;
using RateLimiter.Implementation;
using RateLimiter.Interfaces;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private IRequestLimiterService requestService;
        private string token;
        //[Test]
        //[Theory("api/getFromUS")]
        //public void Example(string apiKey)
        //{
        //    var 

        //    Assert.IsTrue(true);
        //}

        [SetUp]
        public void Startup()
        {
            requestService = new RequestLimiterService(new InMemCacheService());
            token = Guid.NewGuid().ToString();
        }

        [Test]
        public void TestUSRequestBeforeTimeLimit()
        {
            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(requestService.MakeRequest(token, "api/getFromUS"));
            }
            Assert.IsFalse(requestService.MakeRequest(token, "api/getFromUS"));
        }

        [Test]
        public async Task TestUSRequestAfterTimeLimit()
        {
            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(requestService.MakeRequest(token, "api/getFromUS"));
            }
            await Task.Delay(101);
            Assert.IsTrue(requestService.MakeRequest(token, "api/getFromUS"));
        }
        [Test]
        public async Task TestEURequestAfterTimeLimit()
        {
            Assert.IsTrue(requestService.MakeRequest(token, "api/getFromEU"));
            await Task.Delay(11);
            Assert.IsTrue(requestService.MakeRequest(token, "api/getFromEU"));
        }

        [Test]
        public void TestEURequestBeforeTimeLimit()
        {
            Assert.IsTrue(requestService.MakeRequest(token, "api/getFromEU"));
            Assert.IsFalse(requestService.MakeRequest(token, "api/getFromEU"));
        }

        [Test]
        public async Task TestFromGlobalAfterTimeLimit()
        {
            var apiKey = "api/getFromGlobal";
            Assert.IsTrue(requestService.MakeRequest(token, apiKey));
            await Task.Delay(11);
            Assert.IsTrue(requestService.MakeRequest(token, apiKey));
        }


        [Test]
        public void TestFromGlobalBeforeTimeLimit()
        {
            var apiKey = "api/getFromGlobal";
            Assert.IsTrue(requestService.MakeRequest(token, apiKey));
            Assert.IsFalse(requestService.MakeRequest(token, apiKey));
        }

        [Test]
        public void TestFromGlobalCountLimit()
        {
            var apiKey = "api/getFromGlobal";
            for (int i = 0; i < 10; i++)
            {
                requestService.MakeRequest(token, apiKey);
            }
            Assert.IsFalse(requestService.MakeRequest(token, apiKey));
        }

    }
}
