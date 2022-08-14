using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterClientTest
    {
        HttpClient _client;
        RateLimiter _rateLimiter;
        Dictionary<string, Resource> _resources;

        [SetUp]
        public void Init()
        {
            var rule1 = new RequestInTimeSpan(Guid.NewGuid().ToString(), 2, TimeSpan.FromSeconds(30));
            var rule2 = new TimeSpanSinceLast(Guid.NewGuid().ToString(), TimeSpan.FromSeconds(2));

            var resource1 = new Resource("/login", new List<RateLimiterRule>() { rule1 });
            var resource2 = new Resource("/logout", new List<RateLimiterRule>() { rule1, rule2 });

            _resources = new Dictionary<string, Resource>
            {
                { "/login", resource1 },
                { "/logout", resource2 }
            };

            _rateLimiter = new RateLimiter(_resources);
            _client = new HttpClient(_rateLimiter);
            _client.BaseAddress = new Uri("http://localhost");
        }

        [Test(Description = "Doing 2 requests in less than 30 seconds")]
        public async Task RequestToLoginResourceIsValid()
        {
            var isSuccess = true;
            var res = await SendRequest("/login");
            var res2 = await SendRequest("/login");
            isSuccess = isSuccess && res == HttpStatusCode.OK && res2 == HttpStatusCode.OK;
            Assert.IsTrue(isSuccess);
        }

        [Test(Description = "Doing 3 request in less than 30 seconds")]
        public async Task RequestToLoginResourceIsInValid()
        {
            var isSuccess = true;
            var res = await SendRequest("/login");
            var res2 = await SendRequest("/login");
            var res3 = await SendRequest("/login");
            isSuccess = isSuccess
                && res == HttpStatusCode.OK
                && res2 == HttpStatusCode.OK
                && res3 == HttpStatusCode.OK;
            Assert.IsFalse(isSuccess);
        }

        [Test(Description = "Doing 2 requests in less than 30 seconds")]
        public async Task RequestToLogoutResourceIsValid()
        {
            var isSuccess = true;
            var res = await SendRequest("/logout");
            var res2 = await SendRequest("/logout");
            isSuccess = isSuccess && res == HttpStatusCode.OK && res2 == HttpStatusCode.OK;
            Assert.IsTrue(isSuccess);
        }

        [Test(Description = "Doing 3 requests < 30 sec && 3 requests < 2 sec")]
        public async Task RequestToLogoutResourceIsInValid()
        {
            var isSuccess = true;
            var res = await SendRequest("/logout");
            var res2 = await SendRequest("/logout");
            var res3 = await SendRequest("/logout");
            isSuccess = isSuccess
                && res == HttpStatusCode.OK
                && res2 == HttpStatusCode.OK
                && res3 == HttpStatusCode.OK;
            Assert.IsFalse(isSuccess);
        }




        private async Task<HttpStatusCode> SendRequest(string uri)
        {
            try
            {
                var res = await _client.GetAsync(uri);
                return res.StatusCode;
            }
            // Since there is not actual api hosted on localhost, successful requests raise an exception 
            catch (HttpRequestException)
            {
                return HttpStatusCode.OK;
            }
        }
    }
}
