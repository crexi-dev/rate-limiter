using NUnit.Framework;
using RateLimiter.Enums;
using RateLimiter.Models;
using RateLimiter.Rules;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class GeographicBlockRateLimiterRuleTest
    {
        private GeographicBlockRateLimiterRule _rule;

        [SetUp]
        public void Setup()
        {
            _rule = new GeographicBlockRateLimiterRule();
        }

        [Test]
        public void IsRequestAllowed_AllowsRequestIfLocationNotBlocked()
        {
            // Arrange
            var rule = new RuleModel
            {
                Type = RuleType.GeographicBlock,
                Locations = new List<string> { "BlockedLocation1", "BlockedLocation2" }
            };

            var request = new RequestModel
            {
                Source = "Source1",
                UserID = "User1",
                TimeRequested = DateTime.UtcNow,
                Location = "AllowedLocation"
            };

            // Act
            var result = _rule.IsRequestAllowed(request, new List<RequestModel>(), rule);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsRequestAllowed_RejectsRequestIfLocationBlocked()
        {
            // Arrange
            var rule = new RuleModel
            {
                Type = RuleType.GeographicBlock,
                Locations = new List<string> { "BlockedLocation1", "BlockedLocation2" }
            };

            var request = new RequestModel
            {
                Source = "Source1",
                UserID = "User1",
                TimeRequested = DateTime.UtcNow,
                Location = "BlockedLocation1"
            };

            // Act
            var result = _rule.IsRequestAllowed(request, new List<RequestModel>(), rule);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsRequestAllowed_AllowsRequestIfNoLocationsInRule()
        {
            // Arrange
            var rule = new RuleModel
            {
                Type = RuleType.GeographicBlock,
                Locations = null // No locations specified for blocking
            };

            var request = new RequestModel
            {
                Source = "Source1",
                UserID = "User1",
                TimeRequested = DateTime.UtcNow,
                Location = "BlockedLocation1"
            };

            // Act
            var result = _rule.IsRequestAllowed(request, new List<RequestModel>(), rule);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsRequestAllowed_AllowsRequestIfRequestLocationIsEmpty()
        {
            // Arrange
            var rule = new RuleModel
            {
                Type = RuleType.GeographicBlock,
                Locations = new List<string> { "BlockedLocation1", "BlockedLocation2" }
            };

            var request = new RequestModel
            {
                Source = "Source1",
                UserID = "User1",
                TimeRequested = DateTime.UtcNow,
                Location = "" // Empty location
            };

            // Act
            var result = _rule.IsRequestAllowed(request, new List<RequestModel>(), rule);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
