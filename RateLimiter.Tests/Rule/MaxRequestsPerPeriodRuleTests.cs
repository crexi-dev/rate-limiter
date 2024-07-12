using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Model;
using RateLimiter.Rule;

namespace RateLimiter.Tests.Rule
{
    [TestFixture]
    public  class MaxRequestsPerPeriodRuleTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public void CheckRequestAllow_WithZeroConstraints_ReturnsTrue(
            int maxCount, int durationSeconds)
        {
            var requestData = new List<RateLimitRequest>();
            var currentTimestamp = DateTime.UtcNow;

            for (int i = 0; i < 100; i++)
            {
                requestData.Add(new RateLimitRequest
                {
                    Timestamp = currentTimestamp.AddMilliseconds(i),
                    Token = "Test",
                    Url = new Uri("https://www.test.com/test")
                });
            }

            var rule = new MaxRequestsPerPeriodRule(maxCount, TimeSpan.FromSeconds(durationSeconds));

            rule.CheckRequestAllow(requestData).Should().BeTrue();
        }

        [TestCase(10, 100, false)]
        [TestCase(10000, 100, true)]
        public void CheckRequestAllow_WithSpecificMaxCountInFiveSecond_ReturnsExcpectedResponse(
            int maxCount, int numberOfRequest, bool expectedResponse)
        {
            var requestData = new List<RateLimitRequest>();
            var currentTimestamp = DateTime.UtcNow;

            for (int i = 0; i < numberOfRequest; i++)
            {
                requestData.Add(new RateLimitRequest
                {
                    Timestamp = currentTimestamp.AddMilliseconds(i),
                    Token = "Test",
                    Url = new Uri("https://www.test.com/test")
                });
            }

            var rule = new MaxRequestsPerPeriodRule(maxCount, TimeSpan.FromSeconds(5));

            rule.CheckRequestAllow(requestData).Should().Be(expectedResponse);
        }
    }
}
