using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using RateLimiter.Common.Utilities;
using RateLimiter.Models.Domain;

namespace RateLimiter.Filters.Tests
{
    [TestFixture]
    public class CacheClientMetricsFilterTests
    {
        private IDistributedCache _distributedCache;
        private IConfiguration _configuration;
        private const string _accessToken = "12345abcdef";
        [SetUp]
        public void Setup()
        {
            _distributedCache = Substitute.For<IDistributedCache>();
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Interval", "60"}
                })
                .Build();
        }

        [Test]
        public async Task TestSuccessfulCreationOfClientMetrics()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {_accessToken}";

            var fakeActionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var fakeResourceExecutingContext = new ResourceExecutingContext(fakeActionContext, new List<IFilterMetadata>(), new List<IValueProviderFactory>());
            var fakeNextContext = new ResourceExecutedContext(fakeActionContext, new List<IFilterMetadata>());
            var fakeResourceExecutionDelegate = new ResourceExecutionDelegate(() => Task.FromResult(fakeNextContext));

            _distributedCache.GetAsync(Arg.Any<string>()).ReturnsNull();

            //Act
            var cacheClientMetricsFilter = new CacheClientMetricsFilter(_distributedCache, _configuration);
            await cacheClientMetricsFilter.OnResourceExecutionAsync(fakeResourceExecutingContext, fakeResourceExecutionDelegate);

            //Assert
            fakeNextContext.Result.Should().BeNull();
        }

        [Test]
        public async Task TestSuccessfulRemovalOfExpiredTimestamps()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {_accessToken}";

            var fakeActionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var fakeResourceExecutingContext = new ResourceExecutingContext(fakeActionContext, new List<IFilterMetadata>(), new List<IValueProviderFactory>());
            var fakeNextContext = new ResourceExecutedContext(fakeActionContext, new List<IFilterMetadata>());
            var fakeResourceExecutionDelegate = new ResourceExecutionDelegate(() => Task.FromResult(fakeNextContext));

            var interval = _configuration.GetValue<int>("Interval");
            var dateTime = DateTime.UtcNow;
            var clientMetrics = new ClientMetrics(_accessToken, 3, 75L, 6D);
            clientMetrics.Requests.Add(new Request(dateTime, 25L, 2D));
            clientMetrics.Requests.Add(new Request(dateTime.AddSeconds(-interval - 1), 25L, 2D));
            clientMetrics.Requests.Add(new Request(dateTime.AddSeconds(-interval), 25L, 2D));

            _distributedCache.GetAsync(Arg.Any<string>()).Returns(clientMetrics.ToByteArray());

            //Act
            var cacheClientMetricsFilter = new CacheClientMetricsFilter(_distributedCache, _configuration);
            await cacheClientMetricsFilter.OnResourceExecutionAsync(fakeResourceExecutingContext, fakeResourceExecutionDelegate);

            //Assert
            fakeResourceExecutingContext.HttpContext.Items.ContainsKey("DateTime").Should().BeTrue();
            fakeResourceExecutingContext.HttpContext.Items.ContainsKey("ClientMetrics").Should().BeTrue();
            var result = (ClientMetrics)fakeResourceExecutingContext.HttpContext.Items["ClientMetrics"];
            result?.Requests.Count.Should().Be(1);
        }
    }
}
