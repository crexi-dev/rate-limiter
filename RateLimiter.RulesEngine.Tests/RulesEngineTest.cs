using System;
using System.Collections.Generic;
using RateLimiter.RulesEngine.Library.Rules;
using RateLimiter.RulesEngine.Library;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RateLimiter.RulesEngine.Tests
{
    [TestClass]
    public class RulesEngineTest
    {
        [TestMethod]
        public void GetRules_USRule_()
        {
            // arrange
            var serverIP = "183.49.25.23";
            var rules = new List<Rule>()
            {
                new RegionRule(1, "US", RateLimitType.RequestsPerTimespan, RateLimitLevel.Default)
            };

            var fakeRulesRepository = Substitute.For<IRuleRepository>();
            fakeRulesRepository.GetRules(serverIP).Returns(rules);
            var rulesEngine = new RulesEngine(fakeRulesRepository);

            // assert
            var result = rulesEngine.GetRules("fsd");

            // assert
            Assert.AreEqual(1, 1);
        }
    }
}
