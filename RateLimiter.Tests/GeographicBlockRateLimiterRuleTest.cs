using NUnit.Framework;
using RateLimiter.Enums;
using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class GeographicBlockRateLimiterRuleTest
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
        public void ValidateRequest_AllowsRequestIfLocationNotBlocked()
        {
            // Arrange
            var rule = new RuleModel
            {
                Type = RuleType.GeographicBlock,
                Locations = new List<string> { "BlockedLocation1", "BlockedLocation2" }
            };
            _rulesBySource["Source1"] = new List<RuleModel> { rule };

            var request = new RequestModel
            {
                Source = "Source1",
                UserID = "User1",
                TimeRequested = DateTime.UtcNow,
                Location = "AllowedLocation"
            };

            // Act
            var result = _rateLimiterService.ValidateRequest(request);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateRequest_RejectsRequestIfLocationBlocked()
        {
            // Arrange
            var rule = new RuleModel
            {
                Type = RuleType.GeographicBlock,
                Locations = new List<string> { "BlockedLocation1", "BlockedLocation2" }
            };
            _rulesBySource["Source1"] = new List<RuleModel> { rule };

            var request = new RequestModel
            {
                Source = "Source1",
                UserID = "User1",
                TimeRequested = DateTime.UtcNow,
                Location = "BlockedLocation1"
            };

            // Act
            var result = _rateLimiterService.ValidateRequest(request);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ValidateRequest_AllowsRequestIfNoLocationsInRule()
        {
            // Arrange
            var rule = new RuleModel
            {
                Type = RuleType.GeographicBlock,
                Locations = null // No locations specified for blocking
            };
            _rulesBySource["Source1"] = new List<RuleModel> { rule };

            var request = new RequestModel
            {
                Source = "Source1",
                UserID = "User1",
                TimeRequested = DateTime.UtcNow,
                Location = "BlockedLocation1"
            };

            // Act
            var result = _rateLimiterService.ValidateRequest(request);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateRequest_AllowsRequestIfRequestLocationIsEmpty()
        {
            // Arrange
            var rule = new RuleModel
            {
                Type = RuleType.GeographicBlock,
                Locations = new List<string> { "BlockedLocation1", "BlockedLocation2" }
            };
            _rulesBySource["Source1"] = new List<RuleModel> { rule };

            var request = new RequestModel
            {
                Source = "Source1",
                UserID = "User1",
                TimeRequested = DateTime.UtcNow,
                Location = "" // Empty location
            };

            // Act
            var result = _rateLimiterService.ValidateRequest(request);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
