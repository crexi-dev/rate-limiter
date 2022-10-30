using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using RateLimiterApi.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public async Task GetNews1_ReturmsToManyRate_WithRateLimits()
        {
            var webAppFactory = new WebApplicationFactory<Program>();
            var httpClient = webAppFactory.CreateDefaultClient();

            httpClient.DefaultRequestHeaders.Add("x-client-id", "1");
            httpClient.DefaultRequestHeaders.Add("x-client-region", "eu-west-1");

            await httpClient.GetAsync("/news/news-1");
            await httpClient.GetAsync("/news/news-1");
            await httpClient.GetAsync("/news/news-1");
            await httpClient.GetAsync("/news/news-1");
            await httpClient.GetAsync("/news/news-1");
            var responseOk = await httpClient.GetAsync("/news/news-1");
            var responseError = await httpClient.GetAsync("/news/news-1");


            Assert.AreEqual(responseOk.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(responseError.StatusCode, HttpStatusCode.TooManyRequests);
        }

        [Test]
        public async Task GetNews2_ReturmsToManyRate_WithRateLimits()
        {
            var webAppFactory = new WebApplicationFactory<Program>();
            var httpClientEu = webAppFactory.CreateDefaultClient();
            var httpClientUs = webAppFactory.CreateDefaultClient();

            httpClientEu.DefaultRequestHeaders.Add("x-client-id", "1");
            httpClientEu.DefaultRequestHeaders.Add("x-client-region", "eu-west-1");
            httpClientUs.DefaultRequestHeaders.Add("x-client-id", "1");
            httpClientUs.DefaultRequestHeaders.Add("x-client-region", "us-east-1");

            var responseEuOk = await httpClientEu.GetAsync("/news/news-2");
            var responseUsOk = await httpClientUs.GetAsync("/news/news-2");
            var responseEuError = await httpClientEu.GetAsync("/news/news-2");

            Thread.Sleep(6000);

            var responseEuRetryOk = await httpClientEu.GetAsync("/news/news-2");
            var responseUsSecondOk = await httpClientUs.GetAsync("/news/news-2");


            Assert.AreEqual(responseEuOk.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(responseUsOk.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(responseEuError.StatusCode, HttpStatusCode.TooManyRequests);
            Assert.AreEqual(responseEuRetryOk.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(responseUsSecondOk.StatusCode, HttpStatusCode.OK);
        }
    }
}
