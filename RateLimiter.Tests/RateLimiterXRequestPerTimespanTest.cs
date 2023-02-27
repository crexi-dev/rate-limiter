using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterXRequestPerTimespanTest
    {
        private IRateLimiterService _rateLimiterService = default!;
        private readonly string _resource = "/api/comment";

        [OneTimeSetUp]
        public void TestInitialize()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddRateLimiter(options =>
            {
                options.Resource(_resource).Add(new FixedWindowRateLimit
                {
                    // 5 requests per 100 miliseconds
                    PermitLimit = 5,
                    Window = TimeSpan.FromMilliseconds(100)
                });
            });
            var serviceProvider = services.BuildServiceProvider();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();
            _rateLimiterService = serviceProvider.GetRequiredService<IRateLimiterService>();
        }

        [Test]
        public async Task Good()
        {
            var token = $"{Guid.NewGuid()}";
            Assert.That(_rateLimiterService.Validate(_resource, token), Is.True);
            Assert.That(_rateLimiterService.Validate(_resource, token), Is.True);
            Assert.That(_rateLimiterService.Validate(_resource, token), Is.True);
            Assert.That(_rateLimiterService.Validate(_resource, token), Is.True);
            Assert.That(_rateLimiterService.Validate(_resource, token), Is.True);
            await Task.Delay(100);
            Assert.That(_rateLimiterService.Validate(_resource, token), Is.True);
        }
        [Test]
        public async Task Bad()
        {
            var token = $"{Guid.NewGuid()}";
            Assert.That(_rateLimiterService.Validate(_resource, token), Is.True);
            Assert.That(_rateLimiterService.Validate(_resource, token), Is.True);
            Assert.That(_rateLimiterService.Validate(_resource, token), Is.True);
            Assert.That(_rateLimiterService.Validate(_resource, token), Is.True);
            Assert.That(_rateLimiterService.Validate(_resource, token), Is.True);
            await Task.Delay(10);
            Assert.That(_rateLimiterService.Validate(_resource, token), Is.False);
        }
    }
}
