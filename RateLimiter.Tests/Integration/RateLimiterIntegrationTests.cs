using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RateLimiter.Abstractions;
using RateLimiter.Configuration;
using RateLimiter.Extensions;
using RateLimiter.Http;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Integration
{
    [TestFixture]
    public class RateLimiterIntegrationTests
    {
        private IServiceProvider _serviceProvider = null!;
        private DefaultHttpContext _httpContext = null!;
        
        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            
            services.AddRateLimiter(options =>
            {
                var requestCountRule = new RequestCountRule(new Storage.InMemoryRequestTracker())
                {
                    MaxRequests = 3,
                    TimeWindow = TimeSpan.FromSeconds(10)
                };
                
                options.DefaultRules.Add(requestCountRule);
            });
            
            _serviceProvider = services.BuildServiceProvider();
            
            _httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };
            _httpContext.Request.Path = "/api/resource";
            _httpContext.Request.Headers.Add("Authorization", "test-token");
        }
        
        [Test]
        public async Task RateLimiter_AllowsRequestsUpToLimit()
        {
            var middleware = new RateLimiterMiddleware(
                next: (context) => Task.CompletedTask,
                contextFactory: _serviceProvider.GetRequiredService<IRequestContextFactory>()
            );
            
            var options = _serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<RateLimitOptions>>();
            
            for (int i = 0; i < 3; i++)
            {
                await middleware.InvokeAsync(_httpContext, options);
                Assert.That(_httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            }
            
            await middleware.InvokeAsync(_httpContext, options);
            Assert.That(_httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status429TooManyRequests));
        }
        
        [Test]
        public async Task RateLimiter_TracksRequestsPerClientAndResource()
        {
            var middleware = new RateLimiterMiddleware(
                next: (context) => Task.CompletedTask,
                contextFactory: _serviceProvider.GetRequiredService<IRequestContextFactory>()
            );
            
            var options = _serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<RateLimitOptions>>();
            
            for (int i = 0; i < 3; i++)
            {
                _httpContext.Request.Path = "/api/resource1";
                await middleware.InvokeAsync(_httpContext, options);
                Assert.That(_httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            }
            
            _httpContext.Request.Path = "/api/resource1";
            await middleware.InvokeAsync(_httpContext, options);
            Assert.That(_httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status429TooManyRequests));
            
            _httpContext.Request.Path = "/api/resource2";
            _httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await middleware.InvokeAsync(_httpContext, options);
            Assert.That(_httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }
    }
}