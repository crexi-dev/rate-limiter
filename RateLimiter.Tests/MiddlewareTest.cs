using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using RateLimiter.Rules;
using RateLimiter.Rules.FixedWindowRule;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class MiddlewareTest
    {
        private IMemoryCache _cache;

        [SetUp]
        public void Setup()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        [Test]
        public async Task Middleware_AllowsRequest_WhenWithinLimit()
        {
            var options = new FixedWindowRuleOptions
            {
                RuleConditions = null, //rules apply to all requests
                WindowSize = TimeSpan.FromMinutes(1),
                Limit = 5
            };

            var rules = new List<IRateLimitRule> { new FixedWindowRule(options, _cache) };
            var middleware = new RateLimitMiddleware(async (context) => { context.Response.StatusCode = 200; }, rules);

            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
            context.Request.Path = "/test";

            await middleware.InvokeAsync(context);
            Assert.AreEqual(200, context.Response.StatusCode);
        }

        [Test]
        public async Task Middleware_BlocksRequest_WhenLimitExceeded()
        {
            var options = new FixedWindowRuleOptions
            {
                WindowSize = TimeSpan.FromMinutes(2),
                Limit = 1
            };

            var rules = new List<IRateLimitRule> { new FixedWindowRule(options, _cache) };
            var middleware = new RateLimitMiddleware(async (context) => { context.Response.StatusCode = 200; }, rules);

            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
            context.Request.Path = "/test";

            await middleware.InvokeAsync(context);
            Assert.AreEqual(200, context.Response.StatusCode);

            context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
            context.Request.Path = "/test";

            await middleware.InvokeAsync(context);
            Assert.AreEqual(StatusCodes.Status429TooManyRequests, context.Response.StatusCode);
        }
    }
}
