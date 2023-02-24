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
                    TimeSpan = TimeSpan.FromSeconds(1)
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

    }
}
