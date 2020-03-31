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
        public void GetRule_USRule_ResourceId_And_IPAddressMatchesUSRule()
        {
            // arrange
            var resource = "/api/resource1";
            var serverIP = "183.49.25.23";
            var rule = new Rule("US", RateLimitType.RequestsPerTimespan, RateLimitLevel.Default);

            var fakeRulesRepository = Substitute.For<IRuleRepository>();
            fakeRulesRepository.GetRule(resource, serverIP).Returns(rule);
            var rulesEngine = new RulesEngine(fakeRulesRepository, null);

            // assert
            var result = rulesEngine.GetRule(resource, serverIP);

            // assert
            Assert.AreEqual(result, rule);
        }
    }
}
