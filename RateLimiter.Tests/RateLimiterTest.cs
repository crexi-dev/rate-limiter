using System;
using NUnit.Framework;
using RateLimiter.Resources;
using Moq;
using RateLimiter.Model;
using RateLimiter.Repository;
using RateLimiter.Rules;
using RuleEngine;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public void RequetsPerHour_10Min_Success()
        {
            var retrieveTokenInfoMock = new Mock<IRetrieveTokenInfo>(MockBehavior.Strict);
            retrieveTokenInfoMock.Setup(s => s.GetTokenInfo(It.IsAny<string>()))
                .Returns(new TokenInfo()
                    {Location = "US", NoOfTimesCalledInLastHour = 89, LastRequestTime = DateTime.Now.AddHours(-3)});

            #region "RuleEngineFactory can setup rule engine"
            IRulesEngine ruleEngine = new RulesEngine();
            ruleEngine.AddRule(new RuleRequetsPerHour());
            ruleEngine.AddRule(new Rule10MinPassedSinceLastCall());
            #endregion
            IResource resource1 = new Resource1("tkn",ruleEngine,retrieveTokenInfoMock.Object);
            
            var result = resource1.CanContinue();

            Assert.IsTrue(result);
        }

        [Test]
        public void RequetsPerHour_Fail()
        {
            var retrieveTokenInfoMock = new Mock<IRetrieveTokenInfo>(MockBehavior.Strict);
            retrieveTokenInfoMock.Setup(s => s.GetTokenInfo(It.IsAny<string>()))
                .Returns(new TokenInfo()
                    { Location = "US", NoOfTimesCalledInLastHour = 100, LastRequestTime = DateTime.Now.AddHours(-3) });

            IRulesEngine ruleEngine = new RulesEngine();
            ruleEngine.AddRule(new RuleRequetsPerHour());
            IResource resource1 = new Resource1("tkn", ruleEngine, retrieveTokenInfoMock.Object);

            var result = resource1.CanContinue();

            Assert.IsFalse(result);
        }

        [Test]
        public void RuleLocationUS_Success()
        {
            var retrieveTokenInfoMock = new Mock<IRetrieveTokenInfo>(MockBehavior.Strict);
            retrieveTokenInfoMock.Setup(s => s.GetTokenInfo(It.IsAny<string>()))
                .Returns(new TokenInfo()
                    { Location = "US", NoOfTimesCalledInLastHour = 100, LastRequestTime = DateTime.Now.AddHours(-3) });

            IRulesEngine ruleEngine = new RulesEngine();
            ruleEngine.AddRule(new RuleLocationUS());
            IResource resource1 = new Resource1("tkn", ruleEngine, retrieveTokenInfoMock.Object);

            var result = resource1.CanContinue();

            Assert.IsTrue(result);
        }

        [Test]
        public void RuleLocationUS_Fail()
        {
            var retrieveTokenInfoMock = new Mock<IRetrieveTokenInfo>(MockBehavior.Strict);
            retrieveTokenInfoMock.Setup(s => s.GetTokenInfo(It.IsAny<string>()))
                .Returns(new TokenInfo()
                    { Location = "EU", NoOfTimesCalledInLastHour = 100, LastRequestTime = DateTime.Now.AddHours(-3) });

            IRulesEngine ruleEngine = new RulesEngine();
            ruleEngine.AddRule(new RuleLocationUS());
            IResource resource1 = new Resource1("tkn", ruleEngine, retrieveTokenInfoMock.Object);

            var result = resource1.CanContinue();

            Assert.IsFalse(result);
        }

        [Test]
        public void RequetsPerHour_10Min_RuleLocationUS_Success()
        {
            var retrieveTokenInfoMock = new Mock<IRetrieveTokenInfo>(MockBehavior.Strict);
            retrieveTokenInfoMock.Setup(s => s.GetTokenInfo(It.IsAny<string>()))
                .Returns(new TokenInfo()
                    { Location = "US", NoOfTimesCalledInLastHour = 89, LastRequestTime = DateTime.Now.AddHours(-3) });

            IRulesEngine ruleEngine = new RulesEngine();
            ruleEngine.AddRule(new RuleRequetsPerHour());
            ruleEngine.AddRule(new Rule10MinPassedSinceLastCall());
            ruleEngine.AddRule(new RuleLocationUS());
            IResource resource1 = new Resource1("tkn", ruleEngine, retrieveTokenInfoMock.Object);

            var result = resource1.CanContinue();

            Assert.IsTrue(result);
        }

        [Test]
        public void RequetsPerHour_10Min_RuleLocationUS_Fail()
        {
            var retrieveTokenInfoMock = new Mock<IRetrieveTokenInfo>(MockBehavior.Strict);
            retrieveTokenInfoMock.Setup(s => s.GetTokenInfo(It.IsAny<string>()))
                .Returns(new TokenInfo()
                    { Location = "EU", NoOfTimesCalledInLastHour = 89, LastRequestTime = DateTime.Now.AddHours(-3) });

            IRulesEngine ruleEngine = new RulesEngine();
            ruleEngine.AddRule(new RuleRequetsPerHour());
            ruleEngine.AddRule(new Rule10MinPassedSinceLastCall());
            ruleEngine.AddRule(new RuleLocationUS());
            IResource resource1 = new Resource1("tkn", ruleEngine, retrieveTokenInfoMock.Object);

            var result = resource1.CanContinue();

            Assert.IsFalse(result);
        }

    }
}
