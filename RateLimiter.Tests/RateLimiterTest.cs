
using System;
using System.ComponentModel.Design;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RateLimiter.Middleware;
using Xunit;

namespace RateLimiter.Tests
{
    
    public class RateLimiterTest
    {
        private readonly IRateLimitRules _iRateLimitRules = Substitute.For<RateLimitRules>();
        private readonly ILogger<IRateLimitRules> _mockLogger = Substitute.For<ILogger<IRateLimitRules>>();

        private readonly IRateLimitRules _subjectUnderTest;

        public RateLimiterTest()
        {
            _subjectUnderTest = new RateLimitRules(_mockLogger);
        }

        [Fact]
        public void IsValidRequestForUSAKey_ReturnsTrue()
        {
            // ARRANGE
            string key = "IPAddressOFUSA";
            DateTime? apicalltime = DateTime.Now.AddSeconds(6);
            int maxrequests = 5;
            int hitCount = 2;

            // ACT

            var result = _subjectUnderTest.IsValidRequestByKey(apicalltime, key, maxrequests, hitCount);

            // ASSERT
            result.Equals(true);          
        }

        [Fact]
        public void IsValidRequestForUSAKey_ReturnsFalse()
        {
            // ARRANGE
            string key = "IPAddressOFUSA";
            DateTime? apicalltime = DateTime.Now.AddSeconds(2);
            int maxrequests = 5;
            int hitCount = 7;

            // ACT

            var result = _subjectUnderTest.IsValidRequestByKey(apicalltime, key, maxrequests, hitCount);

            // ASSERT
            result.Equals(false);
        }
    }
}
