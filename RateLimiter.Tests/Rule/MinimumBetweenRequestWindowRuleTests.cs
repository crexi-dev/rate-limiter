using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Model;
using RateLimiter.Rule;

namespace RateLimiter.Tests.Rule
{
    [TestFixture]
    public class MinimumBetweenRequestWindowRuleTests
    {
        private readonly List<RateLimitRequest> requestData;
        
        private DateTime currentTimestamp;

        public MinimumBetweenRequestWindowRuleTests()
        {
            this.requestData = new List<RateLimitRequest>();
        }

        [SetUp]
        public void SetUp()
        {
            currentTimestamp = DateTime.UtcNow;
        }

        [TearDown]
        public void TearDown()
        {
            requestData.Clear();
        }

        [Test]
        public void CheckRequestAllow_WithZeroConstraints_ReturnsTrue()
        {
            for (int i = 0; i < 1000; i++)
            {
                requestData.Add(new RateLimitRequest
                {
                    Timestamp = currentTimestamp.AddMilliseconds(i),
                    Token = "Test",
                    Url = new Uri("https://www.test.com/test")
                });
            }

            var rule = new MinimumBetweenRequestWindowRule(TimeSpan.FromSeconds(0));

            rule.CheckRequestAllow(requestData).Should().BeTrue();
        }

        [TestCase(1, 1, true)]
        [TestCase(1, 2, true)]
        [TestCase(2, 1, false)]
        public void CheckRequestAllow_WithSpecificLastRequestGap_ReturnsExcpectedResponse(
            int minimumTimeFromLastRequestInSeconds, int requestGapInSeconds, bool expectedResponse)
        {
            for (int i = 0; i < 3; i++)
            {
                requestData.Add(new RateLimitRequest
                {
                    Timestamp = currentTimestamp.AddSeconds(i * requestGapInSeconds),
                    Token = "Test",
                    Url = new Uri("https://www.test.com/test")
                });
            }

            var rule = new MinimumBetweenRequestWindowRule(TimeSpan.FromSeconds(minimumTimeFromLastRequestInSeconds));

            rule.CheckRequestAllow(requestData).Should().Be(expectedResponse);
        }
    }
}
