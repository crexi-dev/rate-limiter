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
using RateLimiter.RateRules;
using RateLimiter.Contracts;
using RateLimiter.Services;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class TimePassedSinceLastCallRuleTest
    {

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Validate_SingleRequest_true()
        {
            // arrange
            var rule = new TimePassedSinceLastCallRule(new TimeSpan(0, 0, 10));

            var dateMock1 = Mock.Of<ITimeService>(obj => obj.Now == DateTime.Now);

            IRequest request = new Request("Test", "test", "IvanToken", dateMock1, Location.RU);

            // act
            var result = rule.Validate(request);

            // assert
            result.Should().BeTrue();
        }


        [Test]
        public void Validate_Two_Slow_true()
        {
            // arrange
            var rule = new TimePassedSinceLastCallRule(new TimeSpan(0, 0, 10));

            var date = DateTime.Now;
            var dateMock1 = Mock.Of<ITimeService>(obj => obj.Now == date);
            var dateMock2 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(20));

            IRequest request1 = new Request("Test", "test", "IvanToken", dateMock1, Location.RU);
            IRequest request2 = new Request("Test", "test", "IvanToken", dateMock2, Location.RU);

            // act
            var result1 = rule.Validate(request1);
            var result2 = rule.Validate(request2);

            // assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
        }

        [Test]
        public void Validate_Two_Fast_false()
        {
            // arrange
            var rule = new TimePassedSinceLastCallRule(new TimeSpan(0, 0, 10));

            var date = DateTime.Now;
            var dateMock1 = Mock.Of<ITimeService>(obj => obj.Now == date);
            var dateMock2 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(5));

            IRequest request1 = new Request("Test", "test", "IvanToken", dateMock1, Location.RU);
            IRequest request2 = new Request("Test", "test", "IvanToken", dateMock2, Location.RU);

            // act
            var result1 = rule.Validate(request1);
            var result2 = rule.Validate(request2);

            // assert
            result1.Should().BeTrue();
            result2.Should().BeFalse();
        }

        [Test]
        public void Validate_Two_VeryFast_false()
        {
            // arrange
            var rule = new TimePassedSinceLastCallRule(new TimeSpan(0, 0, 10));

            var date = DateTime.Now;
            var dateMock1 = Mock.Of<ITimeService>(obj => obj.Now == date);
            var dateMock2 = Mock.Of<ITimeService>(obj => obj.Now == date.AddMilliseconds(1));
            var dateMock3 = Mock.Of<ITimeService>(obj => obj.Now == date.AddMilliseconds(2));

            IRequest request1 = new Request("Test", "test", "IvanToken", dateMock1, Location.RU);
            IRequest request2 = new Request("Test", "test", "IvanToken", dateMock2, Location.RU);
            IRequest request3 = new Request("Test", "test", "IvanToken", dateMock3, Location.RU);

            // act
            var result1 = rule.Validate(request1);
            var result2 = rule.Validate(request2);
            var result3 = rule.Validate(request3);

            // assert
            result1.Should().BeTrue();
            result2.Should().BeFalse();
            result3.Should().BeFalse();
        }

        [Test]
        public void Validate_Two_TheSameTime_false()
        {
            // arrange
            var rule = new TimePassedSinceLastCallRule(new TimeSpan(0, 0, 10));

            var dateMock = Mock.Of<ITimeService>(obj => obj.Now == DateTime.Now);

            IRequest request1 = new Request("Test", "test1", "IvanToken", dateMock, Location.RU);
            IRequest request2 = new Request("Test", "test2", "IvanToken", dateMock, Location.RU);
            IRequest request3 = new Request("Test", "test3", "IvanToken", dateMock, Location.RU);

            // act
            var result1 = rule.Validate(request1);
            var result2 = rule.Validate(request2);
            var result3 = rule.Validate(request3);

            // assert
            result1.Should().BeTrue();
            result2.Should().BeFalse();
            result3.Should().BeFalse();
        }

        [Test]
        public void Validate_Exception()
        {
            // arrange
            var rule = new TimePassedSinceLastCallRule(new TimeSpan(0, 0, 10));

            // act
            Action act = () => rule.Validate(null);

            // assert
            act.Should().Throw<ArgumentNullException>();
        }

    }
}
