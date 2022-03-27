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
    public class RequestsPerTimespanRuleTest
    {

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Validate_SingleRequest_true()
        {
            // arrange
            var rule = new RequestsPerTimespanRule(new TimeSpan(0, 0, 10), 1);

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
            var rule = new RequestsPerTimespanRule(new TimeSpan(0, 0, 10), 1);

            var date = DateTime.Now;
            var dateMock1 = Mock.Of<ITimeService>(obj => obj.Now == date);
            var dateMock2 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(40));

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
            var rule = new RequestsPerTimespanRule(new TimeSpan(0, 0, 10), 1);

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
        public void Validate_Two_Fast2_true()
        {
            // arrange
            var rule = new RequestsPerTimespanRule(new TimeSpan(0, 0, 10), 1);

            var date = DateTime.Now;
            var dateMock1 = Mock.Of<ITimeService>(obj => obj.Now == date);
            var dateMock2 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(11));

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
        public void Validate_Two_Fast3_true()
        {
            // arrange
            var rule = new RequestsPerTimespanRule(new TimeSpan(0, 0, 10), 1);

            var date = DateTime.Now;
            var dateMock1 = Mock.Of<ITimeService>(obj => obj.Now == date);
            var dateMock2 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(19));

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
        public void Validate_Two_VeryFast_false()
        {
            // arrange
            var rule = new RequestsPerTimespanRule(new TimeSpan(0, 0, 10), 1);

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
            var rule = new RequestsPerTimespanRule(new TimeSpan(0, 0, 10), 1);

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
        public void Validate_5Requests_true()
        {
            // arrange
            var rule = new RequestsPerTimespanRule(new TimeSpan(0, 0, 10), 2);

            var date = DateTime.Now;
            var dateMock1 = Mock.Of<ITimeService>(obj => obj.Now == date);
            var dateMock2 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(1));
            var dateMock3 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(11));
            var dateMock4 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(16));
            var dateMock5 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(21));

            IRequest request1 = new Request("Test", "test", "IvanToken", dateMock1, Location.RU);
            IRequest request2 = new Request("Test", "test", "IvanToken", dateMock2, Location.RU);
            IRequest request3 = new Request("Test", "test", "IvanToken", dateMock3, Location.RU); 
            IRequest request4 = new Request("Test", "test", "IvanToken", dateMock4, Location.RU);
            IRequest request5 = new Request("Test", "test", "IvanToken", dateMock5, Location.RU);

            // act
            var result1 = rule.Validate(request1);
            var result2 = rule.Validate(request2);
            var result3 = rule.Validate(request3);
            var result4 = rule.Validate(request4);
            var result5 = rule.Validate(request5);

            // assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            result3.Should().BeTrue();
            result4.Should().BeTrue();
            result5.Should().BeTrue();
        }
        [Test]
        public void Validate_5Requests()
        {
            // arrange
            var rule = new RequestsPerTimespanRule(new TimeSpan(0, 0, 10), 2);

            var date = DateTime.Now;
            var dateMock1 = Mock.Of<ITimeService>(obj => obj.Now == date);
            var dateMock2 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(1));
            var dateMock3 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(11));
            var dateMock4 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(12));
            var dateMock5 = Mock.Of<ITimeService>(obj => obj.Now == date.AddSeconds(16));

            IRequest request1 = new Request("Test", "test", "IvanToken", dateMock1, Location.RU);
            IRequest request2 = new Request("Test", "test", "IvanToken", dateMock2, Location.RU);
            IRequest request3 = new Request("Test", "test", "IvanToken", dateMock3, Location.RU);
            IRequest request4 = new Request("Test", "test", "IvanToken", dateMock4, Location.RU);
            IRequest request5 = new Request("Test", "test", "IvanToken", dateMock5, Location.RU);

            // act
            var result1 = rule.Validate(request1);
            var result2 = rule.Validate(request2);
            var result3 = rule.Validate(request3);
            var result4 = rule.Validate(request4);
            var result5 = rule.Validate(request5);

            // assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            result3.Should().BeTrue();
            result4.Should().BeFalse();
            result5.Should().BeTrue();
        }

        [Test]
        public void Validate_Exception()
        {
            // arrange
            var rule = new RequestsPerTimespanRule(new TimeSpan(0, 0, 10), 1);

            // act
            Action act = () => rule.Validate(null);

            // assert
            act.Should().Throw<ArgumentNullException>();
        }

    }
}
