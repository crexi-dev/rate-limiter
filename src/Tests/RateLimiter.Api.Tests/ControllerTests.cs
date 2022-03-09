using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NSubstitute;
using NUnit.Framework;
using RateLimiter.Api.Controllers;
using RateLimiter.Api.Attributes;
using RateLimiter.Common.Utilities;
using RateLimiter.Filters;
using RateLimiter.Models.Contract;
using RateLimiter.Models.Domain;
using RateLimiter.Models.Domain.Comparers;
using RateLimiter.Services.Weather;

namespace RateLimiter.Api.Tests
{
    [TestFixture]
    public class Tests
    {
        private IDistributedCache _distributedCache;
        private IConfiguration _configuration;
        private IWeatherForecastService _weatherForecastService;
        private static readonly string[] _summaries = { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        private const string _accessToken = "12345abcdef";

        [SetUp]
        public void Setup()
        {
            _weatherForecastService = new WeatherForecastService();
            _distributedCache = Substitute.For<IDistributedCache>();
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"MaxRequests", "10"},
                    {"MaxSize", "20"},
                    {"MaxRequestUnits", "15"},
                    {"Interval", "60"}
                })
                .Build();
        }

        [Test]
        public async Task TestAllFiltersSuccessful()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {_accessToken}";

            var dateTime = DateTime.UtcNow;
            var clientMetrics = new ClientMetrics(_accessToken, 3, 15L, 6D)
            {
                Requests = new SortedSet<Request>(new TimestampComparer())
                {
                    new (dateTime, 6L, 1D),
                    new (dateTime.AddSeconds(-1), 5L, 2D),
                    new (dateTime.AddSeconds(-2), 4L, 3D)
                }
            };

            var fakeActionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var fakeActionExecutingContext = new ActionExecutingContext(fakeActionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), new object());
            var fakeActionNextContext = new ActionExecutedContext(fakeActionContext, new List<IFilterMetadata>(), new object());
            var fakeActionExecutionDelegate = new ActionExecutionDelegate(() => Task.FromResult(fakeActionNextContext));

            var fakeResourceExecutingContext = new ResourceExecutingContext(fakeActionContext, new List<IFilterMetadata>(), new List<IValueProviderFactory>());
            var fakeResourceNextContext = new ResourceExecutedContext(fakeActionContext, new List<IFilterMetadata>());
            var fakeResourceExecutionDelegate = new ResourceExecutionDelegate(() => Task.FromResult(fakeResourceNextContext));

            _distributedCache.GetAsync(Arg.Any<string>()).Returns(clientMetrics.ToByteArray());

            var fakeAuthFilterContext =
                new AuthorizationFilterContext(fakeActionContext,
                    new List<IFilterMetadata>());

            // Act
            var authorizationFilter = new AuthorizationFilter();
            authorizationFilter.OnAuthorization(fakeAuthFilterContext);

            var cacheClientMetricsFilter = new CacheClientMetricsFilter(_distributedCache, _configuration);
            await cacheClientMetricsFilter.OnResourceExecutionAsync(fakeResourceExecutingContext, fakeResourceExecutionDelegate);

            var maxRequestsThrottleFilter = new MaxRequestsThrottleFilter(_configuration);
            await maxRequestsThrottleFilter.OnActionExecutionAsync(fakeActionExecutingContext, fakeActionExecutionDelegate);

            var controller = new WeatherForecastController(_weatherForecastService);
            var response = controller.GetWeatherForecast();

            // Assert
            response.Should().BeOfType(typeof(OkObjectResult));
            var result = response as OkObjectResult;
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(StatusCodes.Status200OK);

            var weatherForecastResponse = result.Value as WeatherForecastResponse;
            weatherForecastResponse.Should().NotBeNull();
            weatherForecastResponse.Forecasts.Should().NotBeEmpty();
            weatherForecastResponse.Forecasts.Count.Should().BeInRange(1, 6);
            foreach (var forecast in weatherForecastResponse.Forecasts)
            {
                forecast.Summary.Should().NotBeNullOrEmpty();
                forecast.Summary.Should().ContainAny(_summaries);
                forecast.TemperatureC.Should().BeInRange(-20, 56);
                forecast.Date.Should().BeOnOrAfter(DateTime.UtcNow);
            }
        }

        [Test]
        public async Task TestThrottleByMaxRequests()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {_accessToken}";

            var dateTime = DateTime.UtcNow;
            var clientMetrics = new ClientMetrics(_accessToken, 11, 11L, 11D)
            {
                Requests = new SortedSet<Request>(new TimestampComparer())
                {
                    new (dateTime, 1L, 1D),
                    new (dateTime.AddSeconds(-1), 1L, 1D),
                    new (dateTime.AddSeconds(-2), 1L, 1D),
                    new (dateTime.AddSeconds(-3), 1L, 1D),
                    new (dateTime.AddSeconds(-4), 1L, 1D),
                    new (dateTime.AddSeconds(-5), 1L, 1D),
                    new (dateTime.AddSeconds(-6), 1L, 1D),
                    new (dateTime.AddSeconds(-7), 1L, 1D),
                    new (dateTime.AddSeconds(-8), 1L, 1D),
                    new (dateTime.AddSeconds(-9), 1L, 1D),
                    new (dateTime.AddSeconds(-10), 1L, 1D)
                }
            };

            var fakeActionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var fakeActionExecutingContext = new ActionExecutingContext(fakeActionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), new object());
            var fakeActionNextContext = new ActionExecutedContext(fakeActionContext, new List<IFilterMetadata>(), new object());
            var fakeActionExecutionDelegate = new ActionExecutionDelegate(() => Task.FromResult(fakeActionNextContext));

            var fakeResourceExecutingContext = new ResourceExecutingContext(fakeActionContext, new List<IFilterMetadata>(), new List<IValueProviderFactory>());
            var fakeResourceNextContext = new ResourceExecutedContext(fakeActionContext, new List<IFilterMetadata>());
            var fakeResourceExecutionDelegate = new ResourceExecutionDelegate(() => Task.FromResult(fakeResourceNextContext));

            _distributedCache.GetAsync(Arg.Any<string>()).Returns(clientMetrics.ToByteArray());

            var fakeAuthFilterContext =
                new AuthorizationFilterContext(fakeActionContext,
                    new List<IFilterMetadata>());

            // Act
            var authorizationFilter = new AuthorizationFilter();
            authorizationFilter.OnAuthorization(fakeAuthFilterContext);

            var cacheClientMetricsFilter = new CacheClientMetricsFilter(_distributedCache, _configuration);
            await cacheClientMetricsFilter.OnResourceExecutionAsync(fakeResourceExecutingContext, fakeResourceExecutionDelegate);

            var maxRequestsThrottleFilter = new MaxRequestsThrottleFilter(_configuration);
            await maxRequestsThrottleFilter.OnActionExecutionAsync(fakeActionExecutingContext, fakeActionExecutionDelegate);

            //Assert
            fakeActionExecutingContext.Result.Should().BeOfType(typeof(JsonResult));
            fakeActionExecutingContext.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status429TooManyRequests);
        }

        [Test]
        public async Task TestThrottleByMaxSize()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {_accessToken}";

            var dateTime = DateTime.UtcNow;
            var clientMetrics = new ClientMetrics(_accessToken, 9, 29L, 11D)
            {
                Requests = new SortedSet<Request>(new TimestampComparer())
                {
                    new(dateTime, 5L, 1D),
                    new(dateTime.AddSeconds(-1), 4L, 1D),
                    new(dateTime.AddSeconds(-2), 3L, 1D),
                    new(dateTime.AddSeconds(-3), 2L, 1D),
                    new(dateTime.AddSeconds(-4), 1L, 1D),
                    new(dateTime.AddSeconds(-5), 5L, 1D),
                    new(dateTime.AddSeconds(-6), 4L, 1D),
                    new(dateTime.AddSeconds(-7), 3L, 1D),
                    new(dateTime.AddSeconds(-8), 2L, 1D)
                }
            };

            var fakeActionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var fakeActionExecutingContext = new ActionExecutingContext(fakeActionContext, new List<IFilterMetadata>(),
                new Dictionary<string, object>(), new object());
            var fakeActionNextContext =
                new ActionExecutedContext(fakeActionContext, new List<IFilterMetadata>(), new object());
            var fakeActionExecutionDelegate = new ActionExecutionDelegate(() => Task.FromResult(fakeActionNextContext));

            var fakeResourceExecutingContext = new ResourceExecutingContext(fakeActionContext,
                new List<IFilterMetadata>(), new List<IValueProviderFactory>());
            var fakeResourceNextContext = new ResourceExecutedContext(fakeActionContext, new List<IFilterMetadata>());
            var fakeResourceExecutionDelegate =
                new ResourceExecutionDelegate(() => Task.FromResult(fakeResourceNextContext));

            _distributedCache.GetAsync(Arg.Any<string>()).Returns(clientMetrics.ToByteArray());

            var fakeAuthFilterContext =
                new AuthorizationFilterContext(fakeActionContext,
                    new List<IFilterMetadata>());

            // Act
            var authorizationFilter = new AuthorizationFilter();
            authorizationFilter.OnAuthorization(fakeAuthFilterContext);

            var cacheClientMetricsFilter = new CacheClientMetricsFilter(_distributedCache, _configuration);
            await cacheClientMetricsFilter.OnResourceExecutionAsync(fakeResourceExecutingContext,
                fakeResourceExecutionDelegate);

            var maxSizeThrottleFilter = new MaxSizeThrottleFilter(_configuration);
            await maxSizeThrottleFilter.OnActionExecutionAsync(fakeActionExecutingContext, fakeActionExecutionDelegate);

            //Assert
            fakeActionExecutingContext.Result.Should().BeOfType(typeof(JsonResult));
            fakeActionExecutingContext.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status429TooManyRequests);
        }

        [Test]
        public async Task TestThrottleByMaxUnitRequests()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {_accessToken}";

            var dateTime = DateTime.UtcNow;
            var clientMetrics = new ClientMetrics(_accessToken, 9, 9L, 29D)
            {
                Requests = new SortedSet<Request>(new TimestampComparer())
                {
                    new(dateTime, 1L, 5D),
                    new(dateTime.AddSeconds(-1), 1L, 1D),
                    new(dateTime.AddSeconds(-2), 1L, 5D),
                    new(dateTime.AddSeconds(-3), 1L, 1D),
                    new(dateTime.AddSeconds(-4), 1L, 5D),
                    new(dateTime.AddSeconds(-5), 1L, 1D),
                    new(dateTime.AddSeconds(-6), 1L, 5D),
                    new(dateTime.AddSeconds(-7), 1L, 1D),
                    new(dateTime.AddSeconds(-8), 1L, 5D)
                }
            };

            var fakeActionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var fakeActionExecutingContext = new ActionExecutingContext(fakeActionContext, new List<IFilterMetadata>(),
                new Dictionary<string, object>(), new object());
            var fakeActionNextContext =
                new ActionExecutedContext(fakeActionContext, new List<IFilterMetadata>(), new object());
            var fakeActionExecutionDelegate = new ActionExecutionDelegate(() => Task.FromResult(fakeActionNextContext));

            var fakeResourceExecutingContext = new ResourceExecutingContext(fakeActionContext,
                new List<IFilterMetadata>(), new List<IValueProviderFactory>());
            var fakeResourceNextContext = new ResourceExecutedContext(fakeActionContext, new List<IFilterMetadata>());
            var fakeResourceExecutionDelegate =
                new ResourceExecutionDelegate(() => Task.FromResult(fakeResourceNextContext));

            _distributedCache.GetAsync(Arg.Any<string>()).Returns(clientMetrics.ToByteArray());

            var fakeAuthFilterContext =
                new AuthorizationFilterContext(fakeActionContext,
                    new List<IFilterMetadata>());

            // Act
            var authorizationFilter = new AuthorizationFilter();
            authorizationFilter.OnAuthorization(fakeAuthFilterContext);

            var cacheClientMetricsFilter = new CacheClientMetricsFilter(_distributedCache, _configuration);
            await cacheClientMetricsFilter.OnResourceExecutionAsync(fakeResourceExecutingContext,
                fakeResourceExecutionDelegate);

            var maxRequestUnitsThrottleFilter = new MaxRequestUnitsThrottleFilter(_configuration);
            await maxRequestUnitsThrottleFilter.OnActionExecutionAsync(fakeActionExecutingContext, fakeActionExecutionDelegate);

            //Assert
            fakeActionExecutingContext.Result.Should().BeOfType(typeof(JsonResult));
            fakeActionExecutingContext.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status429TooManyRequests);
        }
    }
}