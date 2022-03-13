using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        WebApplicationFactory<Program> application = new WebApplicationFactory<Program>();

        [Test]
        public async Task TestAutoRateLimiterNotInRangeEU()
        {
            var client = application.CreateClient();
            client.DefaultRequestHeaders.Add("Cookie", "AuthToken=Tests");

            var response = await client.GetAsync("/index");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            response = await client.GetAsync("/index");
            Assert.AreEqual(System.Net.HttpStatusCode.TooManyRequests, response.StatusCode);
        }

        [Test]
        public async Task TestAutoRateLimiterInRangeEU()
        {
            var client = application.CreateClient();
            client.DefaultRequestHeaders.Add("Cookie", "AuthToken=Tests1");

            var response = await client.GetAsync("/index");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            
            await Task.Delay(System.TimeSpan.FromSeconds(3));

            response = await client.GetAsync("/index");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task TestAutoRateLimiterInRangeUSA()
        {
            var client = application.CreateClient(new WebApplicationFactoryClientOptions() {
                BaseAddress = new Uri($"https://192.34.56.78:443") 
            });

            client.DefaultRequestHeaders.Add("Cookie", "AuthToken=Tests2");

            var response = await client.GetAsync("/index");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);

            await Task.Delay(System.TimeSpan.FromMilliseconds(3));

            response = await client.GetAsync("/index");
            Assert.AreEqual(System.Net.HttpStatusCode.TooManyRequests, response.StatusCode);
        }

        [Test]
        public async Task TestAutoRateLimiterNotInRangeUSA()
        {
            var client = application.CreateClient(new WebApplicationFactoryClientOptions()
            {
                BaseAddress = new Uri($"https://192.34.56.78:443")
            });

            client.DefaultRequestHeaders.Add("Cookie", "AuthToken=Tests3");

            var response = await client.GetAsync("/index");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);

            await Task.Delay(System.TimeSpan.FromSeconds(3));

            response = await client.GetAsync("/index");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
