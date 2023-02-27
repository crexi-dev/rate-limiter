using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterMixedRulesTest
    {
        private IRateLimiterService _rateLimiterService = default!;
        private readonly string _resource = "/api/share";

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
                    Window = TimeSpan.FromMilliseconds(400)
                }).Add(new FixedWindowRateLimit
                {
                    //10 seconds passed since the last call;
                    PermitLimit = 1,
                    Window = TimeSpan.FromMilliseconds(10)
                });
            });
            var serviceProvider = services.BuildServiceProvider();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();
            _rateLimiterService = serviceProvider.GetRequiredService<IRateLimiterService>();
        }

        [Test]
        public async Task Good()
        {
            var testToken = $"{Guid.NewGuid()}";
            Assert.That(_rateLimiterService.Validate(_resource, testToken), Is.True);
            await Task.Delay(15);
            Assert.That(_rateLimiterService.Validate(_resource, testToken), Is.True);
        }

        [Test]
        public async Task Bad()
        {
            var testToken = $"{Guid.NewGuid()}";
            Assert.That(_rateLimiterService.Validate(_resource, testToken), Is.True);
            await Task.Delay(1);
            Assert.That(_rateLimiterService.Validate(_resource, testToken), Is.False);
        }

        [Test]
        public async Task Mixed()
        {
            var testToken = $"{Guid.NewGuid()}";
            Assert.That(_rateLimiterService.Validate(_resource, testToken), Is.True);
            await Task.Delay(20);
            Assert.That(_rateLimiterService.Validate(_resource, testToken), Is.True);
            await Task.Delay(20);
            Assert.That(_rateLimiterService.Validate(_resource, testToken), Is.True);
            await Task.Delay(20);
            Assert.That(_rateLimiterService.Validate(_resource, testToken), Is.True);
            await Task.Delay(20);
            Assert.That(_rateLimiterService.Validate(_resource, testToken), Is.True);
            await Task.Delay(20);
            Assert.That(_rateLimiterService.Validate(_resource, testToken), Is.False);
        }
    }
}
