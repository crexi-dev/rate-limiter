using NUnit.Framework;
using RateLimiter.Rules;
using RateLimiter.Storage;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class TimeSpanAfterLastCallRuleTests
    {
        [Test]
        public void IsRequestAllowed_NoRequestsForResourceToken_ReturnsTrue()
        {
            // arrange
            string resource = "resource";
            string token = "token";

            var timeSpan = TimeSpan.FromMinutes(1);

            DataStorage.Requests = new Dictionary<string, List<DateTime>>
            {
            };

            var sut = new TimeSpanAfterLastCallRule(timeSpan);

            // act
            var actualResult = sut.IsRequestAllowed(resource, token);

            // assert
            Assert.True(actualResult);
        }

        [Test]
        public void IsRequestAllowed_NoRequestsWithinTimeSpan_ReturnsTrue()
        {
            // arrange
            string resource = "resource";
            string token = "token";
            var key = $"{resource}:{token}";

            var timeSpan = TimeSpan.FromMinutes(1);

            DataStorage.Requests = new Dictionary<string, List<DateTime>>
            {
                { key, new List<DateTime> { DateTime.UtcNow.AddMinutes(-3), DateTime.UtcNow.AddMinutes(-2) } }
            };

            var sut = new TimeSpanAfterLastCallRule(timeSpan);

            // act
            var actualResult = sut.IsRequestAllowed(resource, token);

            // assert
            Assert.True(actualResult);
        }

        [Test]
        public void IsRequestAllowed_RequestExistsWithinTimeSpan_ReturnsFalse()
        {
            // arrange
            string resource = "resource";
            string token = "token";
            var key = $"{resource}:{token}";

            var timeSpan = TimeSpan.FromMinutes(1);

            DataStorage.Requests = new Dictionary<string, List<DateTime>>
            {
                { key, new List<DateTime> { DateTime.UtcNow.AddMinutes(-2), DateTime.UtcNow} }
            };

            var sut = new TimeSpanAfterLastCallRule(timeSpan);

            // act
            var actualResult = sut.IsRequestAllowed(resource, token);

            // assert
            Assert.False(actualResult);
        }
    }
}
