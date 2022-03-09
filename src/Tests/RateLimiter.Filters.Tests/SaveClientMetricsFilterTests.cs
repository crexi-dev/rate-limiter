using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using RateLimiter.Common.Utilities;
using RateLimiter.Models.Domain;
using RateLimiter.Models.Domain.Comparers;

namespace RateLimiter.Filters.Tests
{
    [TestFixture]
    public class SaveClientMetricsFilterTests
    {
        private IDistributedCache _distributedCache;
        private IConfiguration _configuration;
        private const string _accessToken = "12345abcdef";

        [SetUp]
        public void Setup()
        {
            _distributedCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Interval", "60"}
                })
                .Build();
        }

        [Test]
        public async Task TestSuccessfulSavingOfClientMetrics()
        {
            //Arrange
            var dateTime = DateTime.UtcNow;
            var clientMetrics = new ClientMetrics(_accessToken, 6, 21L, 21D)
            {
                Requests = new SortedSet<Request>(new TimestampComparer())
                {
                    new (dateTime, 6L, 1D),
                    new (dateTime.AddSeconds(-1), 5L, 2D),
                    new (dateTime.AddSeconds(-2), 4L, 3D),
                    new (dateTime.AddSeconds(-3), 3L, 4D),
                    new (dateTime.AddSeconds(-4), 2L, 5D),
                    new (dateTime.AddSeconds(-5), 1L, 6D),
                }
            };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {_accessToken}";
            httpContext.Items.Add("DateTime", dateTime);
            httpContext.Items.Add("ClientMetrics", clientMetrics);

            var fakeActionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var fakeActionExecutingContext = new ActionExecutingContext(fakeActionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), new object());
            var fakeNextContext = new ActionExecutedContext(fakeActionContext, new List<IFilterMetadata>(), new object());
            var fakeActionExecutionDelegate = new ActionExecutionDelegate(() => Task.FromResult(fakeNextContext));
            var interval = _configuration.GetValue<int>("Interval");

            //Act
            var saveClientMetricsFilter = new SaveClientMetricsFilter(_distributedCache, _configuration);
            await saveClientMetricsFilter.OnActionExecutionAsync(fakeActionExecutingContext, fakeActionExecutionDelegate);

            //Assert
            fakeNextContext.Result.Should().BeNull();
            var cache = await _distributedCache.GetAsync(_accessToken);
            var response = cache.FromByteArray<ClientMetrics>();
            response.TotalRequests.Should().Be(clientMetrics.TotalRequests);
            response.TotalRequestUnits.Should().Be(clientMetrics.TotalRequestUnits);
            response.TotalSize.Should().Be(clientMetrics.TotalSize);
            response.ExpiresAt.Should().Be(dateTime.AddSeconds(interval));
        }
    }
}
