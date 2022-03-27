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
using RateLimiter.RateRules;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiter_IntegrationТesting
    {
        private IRuleManager _RuleManager;
        private ITimeService _DateService;

        [SetUp]
        public void SetUp()
        {
            _RuleManager = new RuleManager();
            _RuleManager.AddRegionRule(Location.EU, new TimePassedSinceLastCallRule(new TimeSpan(0, 0, 10)));
            _RuleManager.AddRegionRule(Location.US, new TimePassedSinceLastCallRule(new TimeSpan(0, 0, 20)));
            _RuleManager.AddRegionRule(Location.RU, new TimePassedSinceLastCallRule(new TimeSpan(1, 0, 0)));

            _RuleManager.AddResourcesRule("Test1", new RequestsPerTimespanRule(new TimeSpan(0, 1, 0), 3));
            _RuleManager.AddResourcesRule("Test2", new StabRule());
            _RuleManager.AddResourcesRule("Test2", new StabRule());
            _RuleManager.AddResourcesRule("Test3", new StabRule());
            _RuleManager.AddResourcesRule("Test4", new StabRule());

            _DateService = new DateTimeService();
        }

        [Test]
        public void Validate_OneRequest_true()
        {
            // arrange
            IRequest request = new Request("Test1", "test", "IvanToken", _DateService, Location.EU);

            // act
            var result = _RuleManager.Validate(request);

            // assert
            result.Should().BeTrue();
        }

        [Test]
        public void Validate_3Requests_too_fast()
        {
            // arrange
            var date = DateTime.Now;
            var dateMock1 = Mock.Of<ITimeService>(obj => obj.Now == date);
            var dateMock2 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(1));
            var dateMock3 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(2));

            IRequest request1 = new Request("Test1", "test", "IvanToken", dateMock1, Location.EU);
            IRequest request2 = new Request("Test1", "test", "IvanToken", dateMock2, Location.EU);
            IRequest request3 = new Request("Test1", "test", "IvanToken", dateMock3, Location.EU);

            // act
            var result1 = _RuleManager.Validate(request1);
            var result2 = _RuleManager.Validate(request2);
            var result3 = _RuleManager.Validate(request3);

            // assert
            result1.Should().BeTrue();
            result2.Should().BeFalse();
            result3.Should().BeFalse();
        }

        [Test]
        public void Validate_3Requests_too_fast2()
        {
            // arrange
            var date = DateTime.Now;
            var dateMock1 = Mock.Of<ITimeService>(obj => obj.Now == date);
            var dateMock2 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(9));
            var dateMock3 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(11));

            IRequest request1 = new Request("Test1", "test", "IvanToken", dateMock1, Location.EU);
            IRequest request2 = new Request("Test1", "test", "IvanToken", dateMock2, Location.EU);
            IRequest request3 = new Request("Test1", "test", "IvanToken", dateMock3, Location.EU);

            // act
            var result1 = _RuleManager.Validate(request1);
            var result2 = _RuleManager.Validate(request2);
            var result3 = _RuleManager.Validate(request3);

            // assert
            result1.Should().BeTrue();
            result2.Should().BeFalse();
            result3.Should().BeTrue();
        }

        [Test]
        public void Validate_3Requests_too_fast3()
        {
            // arrange
            var date = DateTime.Now;
            var dateMock1 = Mock.Of<ITimeService>(obj => obj.Now == date);
            var dateMock2 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(10));
            var dateMock3 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(11));

            IRequest request1 = new Request("Test1", "test", "IvanToken", dateMock1, Location.EU);
            IRequest request2 = new Request("Test1", "test", "IvanToken", dateMock2, Location.EU);
            IRequest request3 = new Request("Test1", "test", "IvanToken", dateMock3, Location.EU);

            // act
            var result1 = _RuleManager.Validate(request1);
            var result2 = _RuleManager.Validate(request2);
            var result3 = _RuleManager.Validate(request3);

            // assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            result3.Should().BeFalse();
        }

        [Test]
        public void Validate_3Requests()
        {
            // arrange
            var date = DateTime.Now;
            var dateMock1 = Mock.Of<ITimeService>(obj => obj.Now == date);
            var dateMock2 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(10));
            var dateMock3 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(20));

            IRequest request1 = new Request("Test1", "test", "IvanToken", dateMock1, Location.EU);
            IRequest request2 = new Request("Test1", "test", "IvanToken", dateMock2, Location.EU);
            IRequest request3 = new Request("Test1", "test", "IvanToken", dateMock3, Location.EU);

            // act
            var result1 = _RuleManager.Validate(request1);
            var result2 = _RuleManager.Validate(request2);
            var result3 = _RuleManager.Validate(request3);

            // assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            result3.Should().BeTrue();
        }

        [Test]
        public void Validate_4Requests()
        {
            // arrange
            var date = DateTime.Now;
            var dateMock1 = Mock.Of<ITimeService>(obj => obj.Now == date);
            var dateMock2 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(10));
            var dateMock3 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(20));
            var dateMock4 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(40));

            IRequest request1 = new Request("Test1", "test", "IvanToken", dateMock1, Location.EU);
            IRequest request2 = new Request("Test1", "test", "IvanToken", dateMock2, Location.EU);
            IRequest request3 = new Request("Test1", "test", "IvanToken", dateMock3, Location.EU);
            IRequest request4 = new Request("Test1", "test", "IvanToken", dateMock4, Location.EU);

            // act
            var result1 = _RuleManager.Validate(request1);
            var result2 = _RuleManager.Validate(request2);
            var result3 = _RuleManager.Validate(request3);
            var result4 = _RuleManager.Validate(request4);

            // assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            result3.Should().BeTrue();
            result4.Should().BeFalse();
        }
    }
}
