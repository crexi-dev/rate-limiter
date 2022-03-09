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
    public class MaxRequestUnitsThrottleFilterTests
    {
        private IConfiguration _configuration;
        private const string _accessToken = "12345abcdef";

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"MaxRequestUnits", "10"},
                })
                .Build();
        }

        [Test]
        public async Task TestThrottleByMaxRequestUnits()
        {
            //Arrange
            var dateTime = DateTime.UtcNow;
            var clientMetrics = new ClientMetrics(_accessToken, 3, 22L, 11D)
            {
                Requests = new SortedSet<Request>(new TimestampComparer())
                {
                    new (dateTime, 10L, 5D),
                    new (dateTime.AddSeconds(-1), 10L, 5D),
                    new (dateTime.AddSeconds(-2), 2L, 1D),
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
            var maxRequestUnitsThrottleFilter = new MaxRequestUnitsThrottleFilter(_configuration);
            await maxRequestUnitsThrottleFilter.OnActionExecutionAsync(fakeActionExecutingContext, fakeActionExecutionDelegate);

            //Assert
            fakeActionExecutingContext.Result.Should().BeOfType(typeof(JsonResult));
            fakeActionExecutingContext.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status429TooManyRequests);
        }

        [Test]
        public async Task TestNonThrottleByRequestUnits()
        {
            //Arrange
            var dateTime = DateTime.UtcNow;
            var clientMetrics = new ClientMetrics(_accessToken, 3, 22L, 5D)
            {
                Requests = new SortedSet<Request>(new TimestampComparer())
                {
                    new (dateTime, 10L, 2D),
                    new (dateTime.AddSeconds(-1), 10L, 2D),
                    new (dateTime.AddSeconds(-2), 2L, 1D)
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
            var maxRequestUnitsThrottleFilter = new MaxRequestUnitsThrottleFilter(_configuration);
            await maxRequestUnitsThrottleFilter.OnActionExecutionAsync(fakeActionExecutingContext, fakeActionExecutionDelegate);

            //Assert
            fakeNextContext.Result.Should().Be(null);
        }
    }
}
