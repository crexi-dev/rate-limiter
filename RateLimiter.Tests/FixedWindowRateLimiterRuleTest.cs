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
    public class FixedWindowRateLimiterRuleTest
    {
        private RateLimiterService _rateLimiterService;
        private ConcurrentDictionary<string, List<RuleModel>> _rulesBySource;

        [SetUp]
        public void Setup()
        {
            _rulesBySource = new ConcurrentDictionary<string, List<RuleModel>>();
            _rateLimiterService = new RateLimiterService(_rulesBySource);
        }

        [Test]
        public void ValidateRequest_AllowsRequestsWithinLimit()
        {
            // Arrange
            var rule = new RuleModel
            {
                Type = RuleType.FixedWindow,
                Window = TimeSpan.FromSeconds(60),
                MaxRequests = 5,
                Locations = new List<string> { "Location1" }
            };

            _rulesBySource.TryAdd("Source1", new List<RuleModel> { rule });

            var request = new RequestModel
            {
                TimeRequested = DateTime.UtcNow,
                Source = "Source1",
                UserID = "User1",
                Location = "Location1"
            };

            // Act
            var result = _rateLimiterService.ValidateRequest(request);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateRequest_RejectsRequestsExceedingLimit()
        {
            // Arrange
            var rule = new RuleModel
            {
                Type = RuleType.FixedWindow,
                Window = TimeSpan.FromSeconds(60),
                MaxRequests = 2,
                Locations = new List<string> { "Location1" }
            };

            _rulesBySource.TryAdd("Source1", new List<RuleModel> { rule });

            // Simulate existing requests within the window
            var existingRequests = new List<RequestModel>
            {
                new RequestModel { TimeRequested = DateTime.UtcNow.AddSeconds(-10), Location = "Location1", Source = "Source1", UserID = "User1" },
                new RequestModel { TimeRequested = DateTime.UtcNow.AddSeconds(-5), Location = "Location1", Source = "Source1", UserID = "User1" }
            };

            _rateLimiterService = new RateLimiterService(_rulesBySource);
            _rateLimiterService.ValidateRequest(existingRequests[0]);
            _rateLimiterService.ValidateRequest(existingRequests[1]);

            var currentRequest = new RequestModel
            {
                TimeRequested = DateTime.UtcNow,
                Source = "Source1",
                UserID = "User1",
                Location = "Location1"
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
                Type = RuleType.FixedWindow,
                Window = TimeSpan.FromSeconds(60),
                MaxRequests = 5,
                Locations = new List<string> { "Location1" }
            };

            _rulesBySource.TryAdd("Source1", new List<RuleModel> { rule });

            var request = new RequestModel
            {
                TimeRequested = DateTime.UtcNow,
                Source = "Source1",
                UserID = "User1",
                Location = "Location2"
            };

            // Act
            var result = _rateLimiterService.ValidateRequest(request);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateRequest_WithDefaultRule_AddsRuleAndAllowsRequest()
        {
            // Arrange
            var defaultRule = new RuleModel
            {
                Type = RuleType.FixedWindow,
                Window = TimeSpan.FromSeconds(60),
                MaxRequests = 5,
                Locations = new List<string> { "Location1" }
            };

            var request = new RequestModel
            {
                TimeRequested = DateTime.UtcNow,
                Source = "Source1",
                UserID = "User1",
                Location = "Location1"
            };

            // Act
            var result = _rateLimiterService.ValidateRequest(request, defaultRule);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(_rulesBySource.ContainsKey("Source1"));
            Assert.AreEqual(1, _rulesBySource["Source1"].Count);
            Assert.AreEqual(defaultRule, _rulesBySource["Source1"][0]);
        }

        [Test]
        public void ValidateRequest_WithNullRule_ThrowsExceptionIfNoDefaultRule()
        {
            // Arrange
            var request = new RequestModel
            {
                TimeRequested = DateTime.UtcNow,
                Source = "Source1",
                UserID = "User1",
                Location = "Location1"
            };

            // Act & Assert
            var ex = Assert.Throws<ApplicationException>(() => _rateLimiterService.ValidateRequest(request));
            Assert.AreEqual("Cannot find RateLimiterRule for Source1 and a default rule was not provided.", ex.Message);
        }
    }
}
