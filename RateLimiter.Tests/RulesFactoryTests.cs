using NUnit.Framework;
using Moq;
using RateLimiter;
using RateLimiter.Rules;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RulesFactoryTests
    {
        private Mock<IConfigLoader> _mockConfigLoader;
        private Mock<ICacheHelper> _mockCacheHelper;
        private RulesFactory _rulesFactory;

        [SetUp]
        public void SetUp()
        {
            _mockConfigLoader = new Mock<IConfigLoader>();
            _mockCacheHelper = new Mock<ICacheHelper>();
            _rulesFactory = new RulesFactory(_mockConfigLoader.Object, _mockCacheHelper.Object);
        }

        [Test]
        public void GetRule_ShouldReturnEmptyRule_WhenNoRulesAreLoaded()
        {
            // Arrange
            var context = new ContextDto {Id = "test-id", Path = "", Region = "US" };
            _mockConfigLoader.Setup(cl => cl.LoadRules(context)).Returns(new List<RuleDefinition>());

            // Act
            var result = _rulesFactory.GetRule(context);

            // Assert
            Assert.IsInstanceOf<EmptyRule>(result);
        }

        [Test]
        public void GetRule_ShouldReturnRequestsLimitRule_WhenRequestsLimitRuleIsLoaded()
        {
            // Arrange
            var context = new ContextDto {Id = "test-id", Path = "", Region = "US" };
            var rules = new List<RuleDefinition>
            {
                new RuleDefinition { Name = "RequestsLimit", Variables = new Dictionary<string, string>() { { "time", "2000" }, { "requests", "40" } }  }
            };
            _mockConfigLoader.Setup(cl => cl.LoadRules(context)).Returns(rules);

            // Act
            var result = _rulesFactory.GetRule(context);

            // Assert
            Assert.IsInstanceOf<RequestsLimitRule>(result);
        }

        [Test]
        public void GetRule_ShouldReturnTimeLimitRule_WhenTimeLimitRuleIsLoaded()
        {
            // Arrange
            var context = new ContextDto { Id = "test-id", Path = "", Region = "US" };
            var rules = new List<RuleDefinition>
            {
                new RuleDefinition { Name = "TimeLimit", Variables = new Dictionary<string, string>() { { "time", "2000" } } }
            };
            _mockConfigLoader.Setup(cl => cl.LoadRules(context)).Returns(rules);

            // Act
            var result = _rulesFactory.GetRule(context);

            // Assert
            Assert.IsInstanceOf<TimeLimitRule>(result);
        }

    }
}