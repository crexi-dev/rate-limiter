using NUnit.Framework;
using RateLimiter.Api;
using RateLimiter.Common;
using RateLimiter.RateLimiter.Builders;
using RateLimiter.RateLimiter.Rules;
using RateLimiter.RateLimiter.Services;
using RateLimiter.Storage;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public sealed class RateLimiterUsageExample
    {
        [Test]
        public async Task Example()
        {
            var dateTimeProvider = new DateTimeProvider();
            var userRequestRepository = new UserRequestRepository();
            var rateLimitPolicy = new PolicyBuilder()
                .AddPolicy(new RequestAmountPerTimePolicy(10, TimeSpan.FromSeconds(10)))
                .AddPolicy(new RequestTimeAfterLastPolicy(TimeSpan.FromSeconds(20)))
                .Build();
            var rateLimitService = new RateLimitService(
                userRequestRepository, dateTimeProvider, rateLimitPolicy);

            var apiEndpoint = new RandomApiEndpoint();
            var secureApiEndpoint = new SecureApiEndpoint<int>(apiEndpoint, rateLimitService);

            await secureApiEndpoint.ActionAsync("token");
        }
    }
}
