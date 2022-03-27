using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RateLimiterMy.Contracts;
using RateLimiterMy.Models;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using RateLimiterMy.RateRules;
using Moq;
using RateLimiter.Contracts;
using RateLimiter.Services;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RuleManagerTest
    {
        private IRuleManager _RuleManager;
        private ITimeService _DateService;

        [SetUp]
        public void SetUp()
        {
            _RuleManager = new RuleManager();
            _RuleManager.AddRegionRule(Location.EU, new StabRule());
            _RuleManager.AddRegionRule(Location.EU, new StabRule());
            _RuleManager.AddRegionRule(Location.US, new StabRule());
            _RuleManager.AddRegionRule(Location.JP, new StabRule());

            _RuleManager.AddResourcesRule("Test1", new StabRule());
            _RuleManager.AddResourcesRule("Test2", new StabRule());
            _RuleManager.AddResourcesRule("Test2", new StabRule());
            _RuleManager.AddResourcesRule("Test3", new StabRule());
            _RuleManager.AddResourcesRule("Test4", new StabRule());

            _DateService = new DateTimeService();
        }

        [Test]
        public void GetCurrentRules_OneRuleForRegionJP()
        {
            // arrange
            var request = new Request("Test", "test", "IvanToken", _DateService, Location.JP);

            // act
            var result = _RuleManager.GetCurrentRules(request);

            // assert
            result.Count.Should().Be(1);
        }


        [Test]
        public void GetCurrentRules_2RuleForRegionEU_1RuleForResource()
        {
            // arrange
            var request = new Request("Test1", "test", "IvanToken", _DateService, Location.EU);

            // act
            var result = _RuleManager.GetCurrentRules(request);

            // assert
            result.Count.Should().Be(3);
        }

        [Test]
        public void GetCurrentRules_Empty()
        {
            // arrange
            var request = new Request("Test", "test", "IvanToken", _DateService, Location.RU);

            // act
            var result = _RuleManager.GetCurrentRules(request);

            // assert
            result.Should().BeEmpty();
        }

        [Test]
        public void Validate_OneRuleForRegionJP_true()
        {
            // arrange
            var ruleMock = Mock.Of<IRule>(obj => obj.Validate(It.IsAny<IRequest>()) == true);
            _RuleManager.AddRegionRule(Location.RU, ruleMock);

            IRequest request = new Request("Test", "test", "IvanToken", _DateService, Location.RU);

            // act
            var result = _RuleManager.Validate(request);

            // assert
            result.Should().BeTrue();
        }

        [Test]
        public void Validate_OneRuleForRegionJP_false()
        {
            // arrange
            var ruleMock = Mock.Of<IRule>(obj => obj.Validate(It.IsAny<IRequest>()) == false);
            _RuleManager.AddRegionRule(Location.RU, ruleMock);

            IRequest request = new Request("Test", "test", "IvanToken", _DateService, Location.RU);

            // act
            var result = _RuleManager.Validate(request);

            // assert
            result.Should().BeFalse();
        }


        [Test]
        public void Validate_OneRuleForRegionJP_true_MockVerifiable()
        {
            // arrange
            var ruleMock = new Mock<IRule>();
            ruleMock.Setup(x => x.Validate(It.IsAny<IRequest>())).Returns(true).Verifiable();
            _RuleManager.AddRegionRule(Location.RU, ruleMock.Object);

            IRequest request = new Request("Test", "test", "IvanToken", _DateService, Location.RU);

            // act
            var result = _RuleManager.Validate(request);

            // assert
            ruleMock.Verify();
        }

        [Test]
        public void Validate_OneRuleForRegionJP_false_MockVerifiable()
        {
            // arrange
            var ruleMock = new Mock<IRule>();
            ruleMock.Setup(x => x.Validate(It.IsAny<IRequest>())).Returns(false).Verifiable();
            _RuleManager.AddRegionRule(Location.RU, ruleMock.Object);

            IRequest request = new Request("Test", "test", "IvanToken", _DateService, Location.RU);

            // act
            var result = _RuleManager.Validate(request);

            // assert
            ruleMock.Verify();
        }

        [Test]
        public void GetCurrentRules_Null_Exception()
        {
            Action act = () => _RuleManager.GetCurrentRules(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Validate_Null_Exception()
        {
            Action act = () => _RuleManager.Validate(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void AddResourcesRule_NullNameRule_Exception()
        {
            Action act = () => _RuleManager.AddResourcesRule(null, new StabRule());

            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void AddResourcesRule_NullRule_Exception()
        {
            Action act = () => _RuleManager.AddResourcesRule("test", null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void AddRegionRule_NullRegion_Exception()
        {
            Action act = () => _RuleManager.AddRegionRule(0, new StabRule());

            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void AddRegionRule_NullRule_Exception()
        {
            Action act = () => _RuleManager.AddRegionRule(Location.EU, null);

            act.Should().Throw<ArgumentNullException>();
        }
    }
}
