using NUnit.Framework;
using RateLimiter.Rules;
using RateLimiter.Storage;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class FixedRequestNumberRuleTests
    {
        [Test]
        public void IsRequestAllowed_FirstRequestForResourceToken_ReturnsTrue()
        {
            // arrange
            string resource = "resource";
            string token = "token";

            TimeSpan timeSpan = TimeSpan.FromMinutes(1);

            var maxRequestCount = 1;

            DataStorage.Requests = new Dictionary<string, List<DateTime>>
            {
            };

            var sut = new FixedRequestNumberRule(timeSpan, maxRequestCount);

            // act
            var actualResult = sut.IsRequestAllowed(resource, token);

            // assert
            Assert.True(actualResult);
        }

        [Test]
        public void IsRequestAllowed_MaxRequestCountNotExceeded_ReturnsTrue()
        {
            // arrange
            string resource = "resource";
            string token = "token";
            var key = $"{resource}:{token}";

            TimeSpan timeSpan = TimeSpan.FromMinutes(1);

            var maxRequestCount = 2;
            var dateInLatestTimeSpan = DateTime.UtcNow;
            var dateNotInLatestTimeSpan = DateTime.UtcNow.AddMinutes(-2);

            DataStorage.Requests = new Dictionary<string, List<DateTime>>
            {
                { key, new List<DateTime> { dateNotInLatestTimeSpan, dateInLatestTimeSpan } }
            };

            var sut = new FixedRequestNumberRule(timeSpan, maxRequestCount);

            // act
            var actualResult = sut.IsRequestAllowed(resource, token);

            // assert
            Assert.True(actualResult);
        }

        [Test]
        public void IsRequestAllowed_MaxRequestCountExceeded_ReturnsFalse()
        {
            // arrange
            string resource = "resource";
            string token = "token";
            var key = $"{resource}:{token}";

            TimeSpan timeSpan = TimeSpan.FromMinutes(1);
            var maxRequestCount = 1;

            DataStorage.Requests = new Dictionary<string, List<DateTime>>
            {
                { key, new List<DateTime> { DateTime.UtcNow } }
            };

            var sut = new FixedRequestNumberRule(timeSpan, maxRequestCount);

            // act
            var actualResult = sut.IsRequestAllowed(resource, token);

            // assert
            Assert.False(actualResult);
        }
    }
}
