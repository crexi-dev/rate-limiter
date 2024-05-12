using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RateLimiter.Api.Middleware;
using RateLimiter.Api.Models;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimitingMiddlewareTests
    {
        private Mock<RequestDelegate> _nextMock;
        private IOptions<RateLimitingOptions> _options;
        private RateLimitingMiddleware _middleware;

        [SetUp]
        public void Setup()
        {
            _nextMock = new Mock<RequestDelegate>();
            var options = LoadTestOptions();
            _options = Options.Create(options);
            _middleware = new RateLimitingMiddleware(_nextMock.Object, _options);
        }

        private RateLimitingOptions LoadTestOptions()
        {
            var json = File.ReadAllText("testsettings.json");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };
            var config = JsonSerializer.Deserialize<ConfigurationRoot>(json, options);
            return config?.RateLimiting;
        }

        private class ConfigurationRoot
        {
            public RateLimitingOptions RateLimiting { get; set; }
        }

        private HttpContext CreateHttpContext(string token, string country)
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = token;
            context.Request.Headers["X-Country"] = country;
            context.Response.Body = new MemoryStream();
            return context;
        }

        [Test]
        public async Task Request_Within_Limit_Should_Pass()
        {
            var context = CreateHttpContext("valid-token", "US");

            await _middleware.InvokeAsync(context);

            Assert.That(context.Response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task Request_Exceeds_Limit_Should_Throttle()
        {
            var context = CreateHttpContext("throttle-token", "US");

            await _middleware.InvokeAsync(context);
            await _middleware.InvokeAsync(context);
            await _middleware.InvokeAsync(context);
            await _middleware.InvokeAsync(context);

            Assert.That(context.Response.StatusCode, Is.EqualTo(429));
        }

        [Test]
        public async Task EU_Request_Before_Cooldown_Should_Throttle()
        {
            var context = CreateHttpContext("eu-token", "EU");

            await _middleware.InvokeAsync(context);

            var context2 = CreateHttpContext("eu-token", "EU");
            await _middleware.InvokeAsync(context2);

            Assert.That(context2.Response.StatusCode, Is.EqualTo(429));
        }

        [Test]
        public async Task EU_Request_After_Cooldown_Should_Pass()
        {
            var context = CreateHttpContext("eu-token", "EU");

            await _middleware.InvokeAsync(context);

            await Task.Delay(6000);

            var context2 = CreateHttpContext("eu-token", "EU");
            await _middleware.InvokeAsync(context2);

            Assert.That(context2.Response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task Concurrent_Requests_Should_Throttle_Properly()
        {
            var token = "concurrent-token";
            var country = "US";
            var tasks = new List<Task<int>>();

            // Simulate multiple concurrent requests
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var localContext = CreateHttpContext(token, country);
                    await _middleware.InvokeAsync(localContext);
                    return localContext.Response.StatusCode;
                }));
            }

            var results = await Task.WhenAll(tasks);

            // Check that at least some of the requests were throttled
            var throttledRequests = results.Count(statusCode => statusCode == 429);

            Assert.That(throttledRequests, Is.GreaterThan(0));
        }
        
          [Test]
        public async Task Different_Token_Should_Not_Throttle()
        {
            var token1 = "token1";
            var token2 = "token2";
            var country = "US";

            var context1 = CreateHttpContext(token1, country);
            var context2 = CreateHttpContext(token2, country);

            await _middleware.InvokeAsync(context1);
            await _middleware.InvokeAsync(context1);
            await _middleware.InvokeAsync(context2);
            await _middleware.InvokeAsync(context2);

            Assert.That(context1.Response.StatusCode, Is.EqualTo(200));
            Assert.That(context2.Response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task Mixed_Country_Rules_Should_Apply_Correctly()
        {
            var usToken = "us-token";
            var euToken = "eu-token";

            var usContext1 = CreateHttpContext(usToken, "US");
            var usContext2 = CreateHttpContext(usToken, "US");

            var euContext1 = CreateHttpContext(euToken, "EU");
            var euContext2 = CreateHttpContext(euToken, "EU");

            // US requests within limit
            await _middleware.InvokeAsync(usContext1);
            await _middleware.InvokeAsync(usContext1);

            // EU requests should throttle on second request
            await _middleware.InvokeAsync(euContext1);
            await _middleware.InvokeAsync(euContext2);

            Assert.That(usContext1.Response.StatusCode, Is.EqualTo(200));
            Assert.That(euContext2.Response.StatusCode, Is.EqualTo(429));
        }

        [Test]
        public async Task Rapid_Fire_Requests_Should_Throttle()
        {
            var token = "rapid-fire-token";
            var country = "US";
            var context = CreateHttpContext(token, country);

            for (int i = 0; i < 5; i++)
            {
                await _middleware.InvokeAsync(context);
            }

            Assert.That(context.Response.StatusCode, Is.EqualTo(429));
        }
    }
}

