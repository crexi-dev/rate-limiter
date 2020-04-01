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
        public void Evaluate_GetsExpectedConfigSettings_ForResource()
        {
            // arrange
            var resource = "/api/resource1";
            var serverIP = "183.49.25.23";
            var rule = new ResourceRule("/api/resource1 requests per timespan default", resource, RateLimitType.RequestsPerTimespan, RateLimitLevel.Default);
            var expected = new RateLimitSettingsConfig();
            expected[RateLimitType.RequestsPerTimespan] = new RequestsPerTimespanSettings()
            {
                MaxAmount = 5,
                RefillAmount = 5,
                RefillTime = 60
            };

            var expectedSettings = (RequestsPerTimespanSettings) expected[RateLimitType.RequestsPerTimespan];

            var fakeRepository = Substitute.For<IRuleRepository>();
            fakeRepository.GetResourceRule(resource).Returns(rule);

            var fakeRulesEvaluator = Substitute.For<IRulesEvaluator>();
            var rulesEngine = new RulesEngine(fakeRepository, fakeRulesEvaluator);

            // assert
            var result = rulesEngine.Evaluate(resource, serverIP);
            var resultSettings = (RequestsPerTimespanSettings) result[RateLimitType.RequestsPerTimespan];

            // assert
            Assert.AreEqual(resultSettings.MaxAmount, expectedSettings.MaxAmount);
            Assert.AreEqual(resultSettings.RefillAmount, expectedSettings.RefillAmount);
            Assert.AreEqual(resultSettings.RefillTime, expectedSettings.RefillTime);
        }
    }
}
