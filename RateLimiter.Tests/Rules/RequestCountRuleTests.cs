using System;
using System.Threading.Tasks;
using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.Storage;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class RequestCountRuleTests
    {
        private InMemoryRequestTracker _tracker = null!;
        private RequestCountRule _rule = null!;
        
        [SetUp]
        public void Setup()
        {
            _tracker = new InMemoryRequestTracker();
            _rule = new RequestCountRule(_tracker)
            {
                MaxRequests = 3,
                TimeWindow = TimeSpan.FromSeconds(10)
            };
        }
        
        [Test]
        public async Task ValidateAsync_WhenUnderLimit_ReturnsTrue()
        {
            var context = new RequestContext
            {
                ClientToken = "test-token",
                ResourcePath = "/api/resource"
            };
            
            var result1 = await _rule.ValidateAsync(context);
            var result2 = await _rule.ValidateAsync(context);
            var result3 = await _rule.ValidateAsync(context);
            
            Assert.That(result1, Is.True);
            Assert.That(result2, Is.True);
            Assert.That(result3, Is.True);
        }
        
        [Test]
        public async Task ValidateAsync_WhenOverLimit_ReturnsFalse()
        {
            var context = new RequestContext
            {
                ClientToken = "test-token",
                ResourcePath = "/api/resource"
            };
            
            var result1 = await _rule.ValidateAsync(context);
            var result2 = await _rule.ValidateAsync(context);
            var result3 = await _rule.ValidateAsync(context);
            var result4 = await _rule.ValidateAsync(context);
            
            Assert.That(result1, Is.True);
            Assert.That(result2, Is.True);
            Assert.That(result3, Is.True);
            Assert.That(result4, Is.False);
        }
        
        [Test]
        public async Task ValidateAsync_WithDifferentResources_TracksIndependently()
        {
            var context1 = new RequestContext
            {
                ClientToken = "test-token",
                ResourcePath = "/api/resource1"
            };
            
            var context2 = new RequestContext
            {
                ClientToken = "test-token",
                ResourcePath = "/api/resource2"
            };
            
            await _rule.ValidateAsync(context1);
            await _rule.ValidateAsync(context1);
            await _rule.ValidateAsync(context1);
            var result1 = await _rule.ValidateAsync(context1);
            
            var result2 = await _rule.ValidateAsync(context2);
            
            Assert.That(result1, Is.False, "Resource1 should be rate limited");
            Assert.That(result2, Is.True, "Resource2 should not be rate limited");
        }
    }
}