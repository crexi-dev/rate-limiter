using System;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Application;
using RateLimiter.Application.Interfaces;
using RateLimiter.Rules;
using RateLimiter.Tests.Extensions;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class IntervalAccessRuleTests
    {
        private readonly IntervalAccessRule _sut;
        private readonly ICache _cache;
        private readonly TimeSpan _interval;
        private readonly string _resourceName;
        private readonly string _userToken;

        public IntervalAccessRuleTests()
        {
            _resourceName = "TestResource";
            _cache = new ApplicationCache("ApplicationCache");
            _interval = new TimeSpan(0, 0, 0, 1);
            _sut = new IntervalAccessRule(_cache, _interval, _resourceName);
            _userToken = "TestUserJwtToken";
        }

        [SetUp]
        public void Initialization()
        {
            _cache.Clean();
        }

        [Test]
        public void CreateSeveralRequestsWithTheSpecifiedInterval_ValidationShouldPass()
        {
            for (var i = 0; i < 5; i++)
            {
                Thread.Sleep(_interval);
                _sut.Validate(_resourceName, _userToken).Should().BeTrue();
            }
        }

        [Test]
        public void CreateSeveralRequestsWithTheSmallInterval_ValidationShouldFail()
        {
            _sut.Validate(_resourceName, _userToken);
            for (var i = 0; i < 5; i++)
            {
                Thread.Sleep(_interval.AddMilliseconds(-30));
                _sut.Validate(_resourceName, _userToken).Should().BeFalse();
            }
        }

        [Test]
        public void CreateTwoRequestsWithTheSmallInterval_ValidationShouldFail()
        {
            _sut.Validate(_resourceName, _userToken).Should().BeTrue();
            Thread.Sleep(_interval.AddMilliseconds(-30));
            _sut.Validate(_resourceName, _userToken).Should().BeFalse();
        }
    }
}
