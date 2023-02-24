using System;
using System.Security.Authentication.ExtendedProtection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RateLimiter.Configuration;
using RateLimiter.Rules;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public async Task BasicRequestsPerTimeSpanShouldPass()
        {
            var testResource = "test-resource";

            var services = new ServiceCollection();

            services.AddRateLimiter(options =>
            {
                options.For(testResource).AddRule(new RequestsPerTimeSpanRule
                {
                    RequestsCount = 1,
                    TimeSpan = TimeSpan.FromMilliseconds(100)
                });
            });

            var sp = services.BuildServiceProvider();

            var rateLimiter = sp.GetRequiredService<IRateLimiter>();

            var testToken = $"{Guid.NewGuid()}";

            Assert.That(rateLimiter.Check(testResource, testToken), Is.True);
            await Task.Delay(200);
            Assert.That(rateLimiter.Check(testResource, testToken), Is.True);
        }

        [Test]
        public async Task BasicRequestsPerTimeSpanShouldFail()
        {
            var testResource = "test-resource";

            var services = new ServiceCollection();

            services.AddRateLimiter(options =>
            {
                options.For(testResource).AddRule(new RequestsPerTimeSpanRule
                {
                    RequestsCount = 1,
                    TimeSpan = TimeSpan.FromMilliseconds(100)
                });
            });

            var sp = services.BuildServiceProvider();

            var rateLimiter = sp.GetRequiredService<IRateLimiter>();

            var testToken = $"{Guid.NewGuid()}";

            Assert.That(rateLimiter.Check(testResource, testToken), Is.True);
            Assert.That(rateLimiter.Check(testResource, testToken), Is.False);
        }

        [Test]
        public async Task EmptyConfigurationShouldPass()
        {
            var testResource = "test-resource";
            var testToken = $"{Guid.NewGuid()}";

            var services = new ServiceCollection();

            services.AddRateLimiter(_ => { });

            var sp = services.BuildServiceProvider();

            var rateLimiter = sp.GetRequiredService<IRateLimiter>();

            Assert.That(rateLimiter.Check(testResource, testToken), Is.True);
        }

        [Test]
        public async Task BasicTimeFromLastCallShouldPass()
        {
            var testResource = "test-resource";
            var testToken = $"{Guid.NewGuid()}";

            var services = new ServiceCollection();

            services.AddRateLimiter(options =>
            {
                options.For(testResource).AddRule(new TimeFromLastCallRule
                {
                    TimeSpan = TimeSpan.FromMilliseconds(100)
                });
            });
            var sp = services.BuildServiceProvider();

            var rateLimiter = sp.GetRequiredService<IRateLimiter>();

            Assert.That(rateLimiter.Check(testResource, testToken), Is.True);
            await Task.Delay(200);
            Assert.That(rateLimiter.Check(testResource, testToken), Is.True);
        }

        [Test]
        public async Task BasicTimeFromLastCallShouldFail()
        {
            var testResource = "test-resource";
            var testToken = $"{Guid.NewGuid()}";

            var services = new ServiceCollection();

            services.AddRateLimiter(options =>
            {
                options.For(testResource).AddRule(new TimeFromLastCallRule
                {
                    TimeSpan = TimeSpan.FromMilliseconds(100)
                });
            });
            var sp = services.BuildServiceProvider();

            var rateLimiter = sp.GetRequiredService<IRateLimiter>();

            Assert.That(rateLimiter.Check(testResource, testToken), Is.True);
            Assert.That(rateLimiter.Check(testResource, testToken), Is.False);
        }

        [Test]
        public void EmptyResourceNameShouldThrow()
        {
            var testResource = "";

            var services = new ServiceCollection();

            Assert.Throws<ArgumentNullException>(() =>
            {
                services.AddRateLimiter(options =>
                {
                    options.For(testResource);
                });
            });
        }

        [Test]
        public async Task ChainedRulesShouldPass()
        {
            var testResource = "test-resource";

            var services = new ServiceCollection();

            services.AddRateLimiter(options =>
            {
                options.For(testResource)
                    .AddRule(new RequestsPerTimeSpanRule
                    {
                        RequestsCount = 1,
                        TimeSpan = TimeSpan.FromMilliseconds(100)
                    })
                    .AddRule(new TimeFromLastCallRule
                    {
                        TimeSpan = TimeSpan.FromMilliseconds(100)
                    });
            });

            var sp = services.BuildServiceProvider();

            var rateLimiter = sp.GetRequiredService<IRateLimiter>();

            var testToken = $"{Guid.NewGuid()}";

            Assert.That(rateLimiter.Check(testResource, testToken), Is.True);
            await Task.Delay(100);
            Assert.That(rateLimiter.Check(testResource, testToken), Is.True);
        }

        [Test]
        public async Task ChainedRulesShouldFail()
        {
            var testResource = "test-resource";

            var services = new ServiceCollection();

            services.AddRateLimiter(options =>
            {
                options.For(testResource)
                    .AddRule(new RequestsPerTimeSpanRule
                    {
                        RequestsCount = 1,
                        TimeSpan = TimeSpan.FromMilliseconds(100)
                    })
                    .AddRule(new TimeFromLastCallRule
                    {
                        TimeSpan = TimeSpan.FromMilliseconds(200)
                    });
            });

            var sp = services.BuildServiceProvider();

            var rateLimiter = sp.GetRequiredService<IRateLimiter>();

            var testToken = $"{Guid.NewGuid()}";

            Assert.That(rateLimiter.Check(testResource, testToken), Is.True);
            await Task.Delay(100);
            Assert.That(rateLimiter.Check(testResource, testToken), Is.False);
        }

        [Test]
        public async Task ConcurrentAccessShouldPass()
        {
            var testResource1 = "test-resource-1";
            var testResource2 = "test-resource-2";
            var testToken = $"{Guid.NewGuid()}";

            var services = new ServiceCollection();

            services.AddRateLimiter(options =>
            {
                options.For(testResource1).AddRule(new TimeFromLastCallRule
                {
                    TimeSpan = TimeSpan.FromMilliseconds(100)
                });
                options.For(testResource2).AddRule(new TimeFromLastCallRule
                {
                    TimeSpan = TimeSpan.FromMilliseconds(100)
                });
            });
            var sp = services.BuildServiceProvider();

            var rateLimiter = sp.GetRequiredService<IRateLimiter>();

            Assert.That(rateLimiter.Check(testResource1, testToken), Is.True);
            await Task.Delay(100);
            Assert.That(rateLimiter.Check(testResource2, testToken), Is.True);
            await Task.Delay(100);
            Assert.That(rateLimiter.Check(testResource1, testToken), Is.True);
            await Task.Delay(100);
            Assert.That(rateLimiter.Check(testResource2, testToken), Is.True);
        }
    }
}
