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
        public async Task BasicRequestsPerTimestamp()
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

            Assert.That(await rateLimiter.Check(testResource, testToken), Is.True);
        }
    }
}
