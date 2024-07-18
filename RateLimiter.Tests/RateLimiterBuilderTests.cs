using Microsoft.Extensions.Time.Testing;
using RateLimiter.RateLimitingRules;

namespace RateLimiter.Tests
{
    [TestClass]
    public class RateLimiterBuilderTests
    {
        [TestMethod]
        public void RequestsPerTimeMultipleClientsTests()
        {
            TimeProvider timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);

            RateLimiterBuilder<string, string> rateLimiterBuilder = new([]);

            RateLimiter<string, string> rateLimiter =
                rateLimiterBuilder
                .For(x => x.client == "1")
                .Apply(x => [new RequestsPerTimeRule<string, string>(timeProvider, 2, TimeSpan.FromMinutes(1))])
                .For(x => x.client == "2")
                .Apply(x => [new RequestsPerTimeRule<string, string>(timeProvider, 3, TimeSpan.FromMinutes(1))])
                .Build();

            rateLimiter.RegisterRequest("1", "");
            rateLimiter.RegisterRequest("1", "");
            bool hasReachedLimit1 = rateLimiter.HasReachedLimit("1", "");

            rateLimiter.RegisterRequest("2", "");
            rateLimiter.RegisterRequest("2", "");
            bool hasReachedLimit2 = rateLimiter.HasReachedLimit("2", "");

            Assert.IsTrue(hasReachedLimit1);
            Assert.IsFalse(hasReachedLimit2);
        }
    }
}
