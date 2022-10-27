using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RateLimiter.Configs;
using RateLimiter.InMemoryStore;
using RateLimiter.Middleware;
using RateLimiter.Models;
using RateLimiter.Rules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private ICacheStore _cacheStore;
        private RateLimitMiddleware _middleware;
        private IRateLimitService _rateLimiterService;
        private Mock<IOptions<RateLimitConfigurationOptions>> _rules;

        private readonly string newCLientId = Guid.NewGuid().ToString();
        private readonly string clientIdTooManyRequests = Guid.NewGuid().ToString();
        private readonly string clientIdTooSmallInterval = Guid.NewGuid().ToString();

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this._rules = new Mock<IOptions<RateLimitConfigurationOptions>>();

            var rules = new RateLimitConfigurationOptions()
            {
                ExpirationTime = TimeSpan.FromMinutes(10),
                NumberOfRequestsPerTimespanRules = new List<NumberOfRequestsPerTimespan>() {
                    new NumberOfRequestsPerTimespan()
                    {
                        TimeSpan = TimeSpan.FromSeconds(100),
                        Resource = "/test",
                        Method = "DELETE",
                        RequestCount = 1
                    }
                },
                TimeSpanBetweenTwoRequestsRules = new List<TimeSpanBetweenTwoRequests>()
                {
                    new TimeSpanBetweenTwoRequests()
                    {
                        TimeSpan = TimeSpan.FromSeconds(100),
                        Resource = "/test",
                        Method = "DELETE"
                    }
                }
            };

            this._rules.Setup(r => r.Value).Returns(rules);
            var memoryCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

            this._cacheStore = new DistributedCache(memoryCache, this._rules.Object);

            await this.SetMockData();

            this._rateLimiterService = new RateLimitService(_cacheStore, _rules.Object);

            this._middleware = new RateLimitMiddleware(next: (innerHttpContext) =>
            {
                return Task.CompletedTask;
            }, _rateLimiterService);
        }

        [Test]
        public async Task ShouldProcessesRequest_WhenNoMatchingRuleWasFound()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = "/test";
            context.Request.Method = "DELETE";

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, newCLientId) };

            var identity = new ClaimsIdentity(claims);
            context.User = new GenericPrincipal(identity, new string[] { "CLIENT" });

            // Act
            await _middleware.Invoke(context);

            // Assert
            Assert.AreNotEqual(context.Response.StatusCode, (int)HttpStatusCode.TooManyRequests);
        }

        [Test]
        public async Task ShouldBlockRequest_WhenTooManyRequestsAreMadeWithinTimePeriod()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = "/test";
            context.Request.Method = "DELETE";

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, clientIdTooManyRequests) };

            var identity = new ClaimsIdentity(claims);
            context.User = new GenericPrincipal(identity, new string[] { "CLIENT" });

            // Act
            await _middleware.Invoke(context);

            // Assert
            Assert.AreEqual(context.Response.StatusCode, (int)HttpStatusCode.TooManyRequests);
        }

        [Test]
        public async Task ShouldBlockRequest_WhenIntervalBetweenRequestIsSmall()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = "/test";
            context.Request.Method = "DELETE";

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, clientIdTooSmallInterval) };

            var identity = new ClaimsIdentity(claims);
            context.User = new GenericPrincipal(identity, new string[] { "CLIENT" });

            // Act
            await this._middleware.Invoke(context);

            // Assert
            Assert.AreEqual(context.Response.StatusCode, (int)HttpStatusCode.TooManyRequests);
        }


        [Test]
        public async Task ShouldProcessesRequest_ForTheVeryFirstTime()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = "/test";
            context.Request.Method = "DELETE";

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };

            var identity = new ClaimsIdentity(claims);
            context.User = new GenericPrincipal(identity, new string[] { "CLIENT" });

            // Act
            await this._middleware.Invoke(context);

            // Assert
            Assert.AreNotEqual(context.Response.StatusCode, (int)HttpStatusCode.TooManyRequests);
        }

        [Test]
        public async Task ShouldProcessesRequest_WhenCorrespondingMethodRuleNotFound()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = "/test";
            context.Request.Method = "GET";

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, clientIdTooManyRequests) };

            var identity = new ClaimsIdentity(claims);
            context.User = new GenericPrincipal(identity, new string[] { "CLIENT" });

            // Act
            await this._middleware.Invoke(context);

            // Assert
            Assert.AreNotEqual(context.Response.StatusCode, (int)HttpStatusCode.TooManyRequests);
        }

        private async Task SetMockData()
        {
            var clientRequest = new ClientRequest()
            {
                Resource = "/test",
                Method = "DELETE",
                RequestTime = DateTime.UtcNow,
                ClientId = clientIdTooManyRequests,
            };

            for (int i = 0; i < 10; i++)
            {
                await _cacheStore.SetAsync(clientRequest);
            }

            clientRequest.ClientId = clientIdTooSmallInterval;

            await _cacheStore.SetAsync(clientRequest);
        }
    }
}
