using System;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Application;
using RateLimiter.Application.Interfaces;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules
{
    public class PeriodAccessRuleTests
    {
        private readonly PeriodAccessRule _sut;
        private readonly ICache _cache;
        private readonly TimeSpan _period;
        private readonly string _userToken;
        private readonly int _requestsCount;
        private readonly string _resourceName;

        public PeriodAccessRuleTests()
        {
            _resourceName = "TestResource";
            _cache = new ApplicationCache("ApplicationCache");
            _period = new TimeSpan(0, 0, 0, 2);
            _requestsCount = 10;
            _sut = new PeriodAccessRule(_cache, _period, _requestsCount, _resourceName);
            _userToken = "TestUserJwtToken";
        }

        [SetUp]
        public void Initialization()
        {
            _cache.Clean();
        }

        [Test]
        public void AllowedCountRequestsWithPendingDuringThePeriod_ValidationShouldPass()
        {
            var millisecondsBetweenRequests = (int)(_period.TotalMilliseconds / _requestsCount);
            for (var i = 0; i < _requestsCount; i++)
            {
                Thread.Sleep(millisecondsBetweenRequests);
                _sut.Validate(_resourceName, _userToken).Should().BeTrue();
            }
        }

        [Test]
        public void AllowedCountRequestsDuringThePeriod_ValidationShouldPass()
        {
            for (var i = 0; i < _requestsCount; i++)
            {
                _sut.Validate(_resourceName, _userToken).Should().BeTrue();
            }
        }

        [Test]
        public void MoreThenAllowedCountRequestsDuringThePeriod_ValidationShouldFail()
        {
            for (var i = 0; i < _requestsCount; i++)
            {
                _sut.Validate(_resourceName, _userToken).Should().BeTrue();
            }

            // next {_requestCount + 1}th request should fail
            _sut.Validate(_resourceName, _userToken).Should().BeFalse();
        }

        [Test]
        public void SendALotOfRequestsWithAllowablePending_ValidationShouldPass()
        {
            var millisecondsBetweenRequests = (int)(_period.TotalMilliseconds / _requestsCount);
            for (var i = 0; i < 50; i++)
            {
                Thread.Sleep(millisecondsBetweenRequests);
                _sut.Validate(_resourceName, _userToken).Should().BeTrue();
            }
        }
    }
}
