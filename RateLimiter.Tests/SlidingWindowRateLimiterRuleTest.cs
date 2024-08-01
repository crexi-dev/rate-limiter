using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class SlidingWindowRateLimiterRuleTest
    {
        private SlidingWindowRateLimiterRule _rateLimiterRule;

        [SetUp]
        public void Setup()
        {
            _rateLimiterRule = new SlidingWindowRateLimiterRule();
        }

        [Test]
        public void IsRequestAllowed_AllowsRequestsWithinLimit()
        {
            // Arrange
            var requests = new List<RequestModel>
            {
                new RequestModel
                {
                    TimeRequested = DateTime.UtcNow.AddSeconds(-30),
                    Location = "Location1",
                    Source = "Source1",
                    UserID = "User1"
                },
                new RequestModel
                {
                    TimeRequested = DateTime.UtcNow.AddSeconds(-10),
                    Location = "Location1",
                    Source = "Source1",
                    UserID = "User1"
                }
            };
            var rule = new RuleModel
            {
                Window = TimeSpan.FromSeconds(60),
                MaxRequests = 5,
                Locations = new List<string> { "Location1" }
            };
            var currentRequest = new RequestModel
            {
                TimeRequested = DateTime.UtcNow,
                Location = "Location1",
                Source = "Source1",
                UserID = "User1"
            };

            // Act
            var result = _rateLimiterRule.IsRequestAllowed(currentRequest, requests, rule);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsRequestAllowed_RejectsRequestsExceedingLimit()
        {
            // Arrange
            var requests = new List<RequestModel>
            {
                new RequestModel
                {
                    TimeRequested = DateTime.UtcNow.AddSeconds(-10),
                    Location = "Location1",
                    Source = "Source1",
                    UserID = "User1"
                },
                new RequestModel
                {
                    TimeRequested = DateTime.UtcNow.AddSeconds(-5),
                    Location = "Location1",
                    Source = "Source1",
                    UserID = "User1"
                },
                new RequestModel
                {
                    TimeRequested = DateTime.UtcNow.AddSeconds(-2),
                    Location = "Location1",
                    Source = "Source1",
                    UserID = "User1"
                }
            };
            var rule = new RuleModel
            {
                Window = TimeSpan.FromSeconds(60),
                MaxRequests = 3,
                Locations = new List<string> { "Location1" }
            };
            var currentRequest = new RequestModel
            {
                TimeRequested = DateTime.UtcNow,
                Location = "Location1",
                Source = "Source1",
                UserID = "User1"
            };

            // Act
            var result = _rateLimiterRule.IsRequestAllowed(currentRequest, requests, rule);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsRequestAllowed_AllowsRequestsWithDifferentLocation()
        {
            // Arrange
            var requests = new List<RequestModel>
            {
                new RequestModel
                {
                    TimeRequested = DateTime.UtcNow.AddSeconds(-30),
                    Location = "Location1",
                    Source = "Source1",
                    UserID = "User1"
                }
            };
            var rule = new RuleModel
            {
                Window = TimeSpan.FromSeconds(60),
                MaxRequests = 5,
                Locations = new List<string> { "Location1" }
            };
            var currentRequest = new RequestModel
            {
                TimeRequested = DateTime.UtcNow,
                Location = "Location2",
                Source = "Source1",
                UserID = "User1"
            };

            // Act
            var result = _rateLimiterRule.IsRequestAllowed(currentRequest, requests, rule);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsRequestAllowed_WithNullLocationInRule_AllowsRequestsFromAnyLocation()
        {
            // Arrange
            var requests = new List<RequestModel>
            {
                new RequestModel
                {
                    TimeRequested = DateTime.UtcNow.AddSeconds(-30),
                    Location = "Location1",
                    Source = "Source1",
                    UserID = "User1"
                }
            };
            var rule = new RuleModel
            {
                Window = TimeSpan.FromSeconds(60),
                MaxRequests = 5,
                Locations = null
            };
            var currentRequest = new RequestModel
            {
                TimeRequested = DateTime.UtcNow,
                Location = "Location2",
                Source = "Source1",
                UserID = "User1"
            };

            // Act
            var result = _rateLimiterRule.IsRequestAllowed(currentRequest, requests, rule);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsRequestAllowed_WithExpiredRequests_RemovesExpiredRequests()
        {
            // Arrange
            var requests = new List<RequestModel>
            {
                new RequestModel
                {
                    TimeRequested = DateTime.UtcNow.AddSeconds(-70),
                    Location = "Location1",
                    Source = "Source1",
                    UserID = "User1"
                }, // Expired request
                new RequestModel
                {
                    TimeRequested = DateTime.UtcNow.AddSeconds(-10),
                    Location = "Location1",
                    Source = "Source1",
                    UserID = "User1"
                }
            };
            var rule = new RuleModel
            {
                Window = TimeSpan.FromSeconds(60),
                MaxRequests = 5,
                Locations = new List<string> { "Location1" }
            };
            var currentRequest = new RequestModel
            {
                TimeRequested = DateTime.UtcNow,
                Location = "Location1",
                Source = "Source1",
                UserID = "User1"
            };

            // Act
            var result = _rateLimiterRule.IsRequestAllowed(currentRequest, requests, rule);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
