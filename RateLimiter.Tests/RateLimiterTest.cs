using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RateLimiter.Constants;
using RateLimiter.DataCaching;
using RateLimiter.Enums;
using RateLimiter.Interfaces;
using RateLimiter.Middleware;
using RateLimiter.Repositories;
using RateLimiter.Services;
using System.IO;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private ICacheService _cacheService;
        private IRulesRepository _rulesRepository;
        private IRateLimiterService _rateLimiterService;
        private RateLimitingMiddleware _middlewareInstance;

        [OneTimeSetUp]
        public void TestInitialize()
        {
            var services = RegisterDependencies();

            var serviceProvider = services.BuildServiceProvider();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();

            _cacheService = new CacheService(memoryCache);
            _rulesRepository = new RulesRepository();
            _rateLimiterService = new RateLimiterService(_cacheService, _rulesRepository);

            _middlewareInstance = new RateLimitingMiddleware(httpContext =>
            {
                return Task.CompletedTask;
            }, _rateLimiterService);
        }

        [Test]
        public async Task GetProducts_With_RateLimits_US()
        {
            DefaultHttpContext ctx = new();
            ctx.Response.Body = new MemoryStream();
            ctx.Request.Path = EndpointConstatns.GetProducts;
            ctx.Request.Method = "GET";
            ctx.Items.Add("Location", Location.US);

            await _middlewareInstance.InovkeAsync(ctx); 
        }

        [Test]
        public async Task GetProducts_With_RateLimits_EU()
        {
            DefaultHttpContext ctx = new();
            ctx.Response.Body = new MemoryStream();
            ctx.Request.Path = EndpointConstatns.GetProducts;
            ctx.Request.Method = "GET";
            ctx.Items.Add("Location", Location.EU);

            await _middlewareInstance.InovkeAsync(ctx);
        }

        [Test]
        public async Task GetCustomers_With_RateLimits_EU()
        {
            DefaultHttpContext ctx = new();
            ctx.Response.Body = new MemoryStream();
            ctx.Request.Path = EndpointConstatns.GetCustomers;
            ctx.Request.Method = "GET";
            ctx.Items.Add("Location", Location.EU);

            await _middlewareInstance.InovkeAsync(ctx);
        }

        [Test]
        public async Task GetCustomers_With_RateLimits_US()
        {
            DefaultHttpContext ctx = new();
            ctx.Response.Body = new MemoryStream();
            ctx.Request.Path = EndpointConstatns.GetCustomers;
            ctx.Request.Method = "GET";
            ctx.Items.Add("Location", Location.US);

            await _middlewareInstance.InovkeAsync(ctx);
        }

        [Test]
        public async Task GetEmployees_With_RateLimits_US()
        {
            DefaultHttpContext ctx = new();
            ctx.Response.Body = new MemoryStream();
            ctx.Request.Path = EndpointConstatns.GetEmployees;
            ctx.Request.Method = "GET";
            ctx.Items.Add("Location", Location.US);

            await _middlewareInstance.InovkeAsync(ctx);
        }

        [Test]
        public async Task GetEmployees_With_RateLimits_EU()
        {
            DefaultHttpContext ctx = new();
            ctx.Response.Body = new MemoryStream();
            ctx.Request.Path = EndpointConstatns.GetEmployees;
            ctx.Request.Method = "GET";
            ctx.Items.Add("Location", Location.EU);

            await _middlewareInstance.InovkeAsync(ctx);
        }

        private IServiceCollection RegisterDependencies()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddMemoryCache();

            return services;
        }
    }
}
