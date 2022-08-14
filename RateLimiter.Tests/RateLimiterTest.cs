using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public async Task GetResourceIsNull()
        {
            RateLimiter rateLimiter = new RateLimiter(new Dictionary<string, Resource>());
            rateLimiter.GetResourceId = null;
            HttpClient _client = new HttpClient(rateLimiter);
            _client.BaseAddress = new Uri("http://localhost");

            Exception? e = null;
            try
            {
                await _client.GetAsync("/login");
            }
            catch (Exception ex)
            {
                e = ex;
            }
            Assert.IsNotNull(e);
        }

        [Test]
        public void GetResourceIsNotNull()
        {
            RateLimiter rateLimiter = new RateLimiter(new Dictionary<string, Resource>());
            Assert.IsNotNull(rateLimiter.GetResourceId);
        }

        [Test]
        public void GetValidResource()
        {
            var res = "/login";
            RateLimiter rateLimiter = new RateLimiter(new Dictionary<string, Resource>());
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost" + res));
            var resource = rateLimiter.GetResourceId(request);
            Assert.AreEqual(res, resource);
        }

    }
}
