using RateLimiter.Library;
using RateLimiter.RulesEngine.Library.Rules;
using RateLimiter.RulesEngine.Library;
using NSubstitute;
using NUnit.Framework;

namespace RateLimiter.RulesEngine.Tests
{
    [TestFixture]
    public class RulesEngineTest
    {
        [Test]
        public void Evaluate_GetsExpectedConfigSettings_ForGlobalResourceRule()
        {
            // arrange
            var resource = "/api/resource1";
            var serverIP = "172.39.67.32";

            var resourceRule = new ResourceRule("/api/resource1 : all regions : requests per timespan : default settings",
                resource,
                Region.All,
                RateLimitType.RequestsPerTimespan,
                RateLimitLevel.Default);

            var regionRule = new RegionRule("US",
                RateLimitType.TimespanPassedSinceLastCall,
                RateLimitLevel.Low);

            var expected = new RateLimitSettingsConfig();
            expected[RateLimitType.RequestsPerTimespan] = new TokenBucketSettings()
            {
                MaxAmount = 5,
                RefillAmount = 5,
                RefillTime = 60
            };

            var expectedSettings = (TokenBucketSettings) expected[RateLimitType.RequestsPerTimespan];

            var fakeRepository = Substitute.For<IRuleRepository>();
            var fakeCache = Substitute.For<IRuleCache>();
            fakeCache["/api/resource1All"].Returns(resourceRule);
            fakeCache["US"].Returns(regionRule);

            var rulesEngine = new RulesEngine(fakeRepository, fakeCache);

            // act
            var result = rulesEngine.Evaluate(resource, serverIP);
            var resultSettings = (TokenBucketSettings) result[RateLimitType.RequestsPerTimespan];

            // assert
            Assert.AreEqual(resultSettings.MaxAmount, expectedSettings.MaxAmount);
            Assert.AreEqual(resultSettings.RefillAmount, expectedSettings.RefillAmount);
            Assert.AreEqual(resultSettings.RefillTime, expectedSettings.RefillTime);
        }
    }
}
