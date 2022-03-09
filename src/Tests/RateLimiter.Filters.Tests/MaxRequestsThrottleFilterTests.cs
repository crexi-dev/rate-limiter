using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using RateLimiter.Models.Domain;
using RateLimiter.Models.Domain.Comparers;

namespace RateLimiter.Filters.Tests
{
    [TestFixture]
    public class MaxRequestsThrottleFilterTests
    {
        private IConfiguration _configuration;
        private const string _accessToken = "12345abcdef";

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"MaxRequests", "5"},
                })
                .Build();
        }

        [Test]
        public async Task TestThrottleByMaxRequests()
        {
            //Arrange
            var dateTime = DateTime.UtcNow;
            var clientMetrics = new ClientMetrics(_accessToken, 6, 150L, 6D)
            {
                Requests = new SortedSet<Request>(new TimestampComparer())
                {
                    new (dateTime, 25L, 1D),
                    new (dateTime.AddSeconds(-1), 25L, 1D),
                    new (dateTime.AddSeconds(-2), 25L, 1D),
                    new (dateTime.AddSeconds(-3), 25L, 1D),
                    new (dateTime.AddSeconds(-4), 25L, 1D),
                    new (dateTime.AddSeconds(-5), 25L, 1D),
                }
            };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {_accessToken}";
            httpContext.Items.Add("ClientMetrics", clientMetrics);

            var fakeActionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var fakeActionExecutingContext = new ActionExecutingContext(fakeActionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), new object());
            var fakeNextContext = new ActionExecutedContext(fakeActionContext, new List<IFilterMetadata>(), new object());
            var fakeActionExecutionDelegate = new ActionExecutionDelegate(() => Task.FromResult(fakeNextContext));

            //Act
            var maxRequestsThrottleFilter = new MaxRequestsThrottleFilter(_configuration);
            await maxRequestsThrottleFilter.OnActionExecutionAsync(fakeActionExecutingContext, fakeActionExecutionDelegate);

            //Assert
            fakeActionExecutingContext.Result.Should().BeOfType(typeof(JsonResult));
            fakeActionExecutingContext.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status429TooManyRequests);
        }

        [Test]
        public async Task TestNonThrottleByMaxRequests()
        {
            //Arrange
            var dateTime = DateTime.UtcNow;
            var clientMetrics = new ClientMetrics(_accessToken, 6, 150L, 6D)
            {
                Requests = new SortedSet<Request>(new TimestampComparer())
                {
                    new (dateTime, 25L, 1D),
                    new (dateTime.AddSeconds(-1), 25L, 1D),
                    new (dateTime.AddSeconds(-2), 25L, 1D),
                    new (dateTime.AddSeconds(-3), 25L, 1D),
                    new (dateTime.AddSeconds(-4), 25L, 1D),
                    new (dateTime.AddSeconds(-5), 25L, 1D),
                }
            };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {_accessToken}";
            httpContext.Items.Add("ClientMetrics", clientMetrics);

            var fakeActionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var fakeActionExecutingContext = new ActionExecutingContext(fakeActionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), new object());
            var fakeNextContext = new ActionExecutedContext(fakeActionContext, new List<IFilterMetadata>(), new object());
            var fakeActionExecutionDelegate = new ActionExecutionDelegate(() => Task.FromResult(fakeNextContext));

            //Act
            var maxRequestsThrottleFilter = new MaxRequestsThrottleFilter(_configuration);
            await maxRequestsThrottleFilter.OnActionExecutionAsync(fakeActionExecutingContext, fakeActionExecutionDelegate);

            //Assert
            fakeNextContext.Result.Should().Be(null);
        }
    }
}
