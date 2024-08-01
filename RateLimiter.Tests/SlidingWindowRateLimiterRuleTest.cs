using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Services;
using RateLimiter.Rules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using RateLimiter.Enums;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class SlidingWindowRateLimiterRuleTest
    {
        private RateLimiterService _rateLimiterService;
        private ConcurrentDictionary<string, List<RuleModel>> _rulesBySource;

        [SetUp]
        public void Setup()
        {
            // Initialize rules and service
            _rulesBySource = new ConcurrentDictionary<string, List<RuleModel>>();
            _rateLimiterService = new RateLimiterService(_rulesBySource);
        }

        [Test]
        public void ValidateRequest_AllowsRequestsWithinLimit()
        {
            // Arrange
            var rule = new RuleModel
            {
                Window = TimeSpan.FromSeconds(60),
                MaxRequests = 5,
                Locations = new List<string> { "Location1" },
                Type = RuleType.SlidingWindow
            };
            _rulesBySource["Source1"] = new List<RuleModel> { rule };

            var currentRequest = new RequestModel
            {
                TimeRequested = DateTime.UtcNow,
                Location = "Location1",
                Source = "Source1",
                UserID = "User1"
            };

            // Act
            var result = _rateLimiterService.ValidateRequest(currentRequest);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateRequest_RejectsRequestsExceedingLimit()
        {
            // Arrange
            var rule = new RuleModel
            {
                Window = TimeSpan.FromSeconds(60),
                MaxRequests = 3,
                Locations = new List<string> { "Location1" },
                Type = RuleType.SlidingWindow
            };
            _rulesBySource["Source1"] = new List<RuleModel> { rule };

            // Simulate past requests
            var requests = new List<RequestModel>
            {
                new RequestModel { TimeRequested = DateTime.UtcNow.AddSeconds(-10), Location = "Location1", Source = "Source1", UserID = "User1" },
                new RequestModel { TimeRequested = DateTime.UtcNow.AddSeconds(-5), Location = "Location1", Source = "Source1", UserID = "User1" },
                new RequestModel { TimeRequested = DateTime.UtcNow.AddSeconds(-2), Location = "Location1", Source = "Source1", UserID = "User1" }
            };
            foreach (var req in requests)
            {
                _rateLimiterService.ValidateRequest(req); // Ensure these requests are added
            }

            var currentRequest = new RequestModel
            {
                TimeRequested = DateTime.UtcNow,
                Location = "Location1",
                Source = "Source1",
                UserID = "User1"
            };

            // Act
            var result = _rateLimiterService.ValidateRequest(currentRequest);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ValidateRequest_AllowsRequestsWithDifferentLocation()
        {
            // Arrange
            var rule = new RuleModel
            {
                Window = TimeSpan.FromSeconds(60),
                MaxRequests = 5,
                Locations = new List<string> { "Location1" },
                Type = RuleType.SlidingWindow
            };
            _rulesBySource["Source1"] = new List<RuleModel> { rule };

            var pastRequest = new RequestModel
            {
                TimeRequested = DateTime.UtcNow.AddSeconds(-30),
                Location = "Location1",
                Source = "Source1",
                UserID = "User1"
            };
            _rateLimiterService.ValidateRequest(pastRequest);

            var currentRequest = new RequestModel
            {
                TimeRequested = DateTime.UtcNow,
                Location = "Location2",
                Source = "Source1",
                UserID = "User1"
            };

            // Act
            var result = _rateLimiterService.ValidateRequest(currentRequest);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateRequest_WithNullLocationInRule_AllowsRequestsFromAnyLocation()
        {
            // Arrange
            var rule = new RuleModel
            {
                Window = TimeSpan.FromSeconds(60),
                MaxRequests = 5,
                Locations = null,
                Type = RuleType.SlidingWindow
            };
            _rulesBySource["Source1"] = new List<RuleModel> { rule };

            var pastRequest = new RequestModel
            {
                TimeRequested = DateTime.UtcNow.AddSeconds(-30),
                Location = "Location1",
                Source = "Source1",
                UserID = "User1"
            };
            _rateLimiterService.ValidateRequest(pastRequest);

            var currentRequest = new RequestModel
            {
                TimeRequested = DateTime.UtcNow,
                Location = "Location2",
                Source = "Source1",
                UserID = "User1"
            };

            // Act
            var result = _rateLimiterService.ValidateRequest(currentRequest);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateRequest_WithExpiredRequests_RemovesExpiredRequests()
        {
            // Arrange
            var rule = new RuleModel
            {
                Window = TimeSpan.FromSeconds(60),
                MaxRequests = 5,
                Locations = new List<string> { "Location1" },
                Type = RuleType.SlidingWindow
            };
            _rulesBySource["Source1"] = new List<RuleModel> { rule };

            // Simulate past requests
            var requests = new List<RequestModel>
            {
                new RequestModel { TimeRequested = DateTime.UtcNow.AddSeconds(-70), Location = "Location1", Source = "Source1", UserID = "User1" }, // Expired request
                new RequestModel { TimeRequested = DateTime.UtcNow.AddSeconds(-10), Location = "Location1", Source = "Source1", UserID = "User1" }
            };
            foreach (var req in requests)
            {
                _rateLimiterService.ValidateRequest(req); // Ensure these requests are added
            }

            var currentRequest = new RequestModel
            {
                TimeRequested = DateTime.UtcNow,
                Location = "Location1",
                Source = "Source1",
                UserID = "User1"
            };

            // Act
            var result = _rateLimiterService.ValidateRequest(currentRequest);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
