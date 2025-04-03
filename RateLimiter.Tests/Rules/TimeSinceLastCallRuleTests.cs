using System;
using System.Threading.Tasks;
using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.Storage;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class TimeSinceLastCallRuleTests
    {
        private InMemoryRequestTracker _tracker = null!;
        private TimeSinceLastCallRule _rule = null!;
        
        [SetUp]
        public void Setup()
        {
            _tracker = new InMemoryRequestTracker();
            _rule = new TimeSinceLastCallRule(_tracker)
            {
                MinInterval = TimeSpan.FromSeconds(1)
            };
        }
        
        [Test]
        public async Task ValidateAsync_FirstRequest_ReturnsTrue()
        {
            var context = new RequestContext
            {
                ClientToken = "test-token",
                ResourcePath = "/api/resource"
            };
            
            var result = await _rule.ValidateAsync(context);
            
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task ValidateAsync_SecondRequestTooSoon_ReturnsFalse()
        {
            var context = new RequestContext
            {
                ClientToken = "test-token",
                ResourcePath = "/api/resource"
            };
            
            var result1 = await _rule.ValidateAsync(context);
            var result2 = await _rule.ValidateAsync(context);
            
            Assert.That(result1, Is.True);
            Assert.That(result2, Is.False);
        }
        
        [Test]
        public async Task ValidateAsync_SecondRequestAfterInterval_ReturnsTrue()
        {
            var context = new RequestContext
            {
                ClientToken = "test-token",
                ResourcePath = "/api/resource"
            };
            
            var result1 = await _rule.ValidateAsync(context);
            
            await Task.Delay(TimeSpan.FromSeconds(1.1));
            
            var result2 = await _rule.ValidateAsync(context);
            
            Assert.That(result1, Is.True);
            Assert.That(result2, Is.True);
        }
        
        [Test]
        public async Task ValidateAsync_DifferentResources_TrackedIndependently()
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
            
            var result1 = await _rule.ValidateAsync(context1);
            var result2 = await _rule.ValidateAsync(context2);
            
            Assert.That(result1, Is.True);
            Assert.That(result2, Is.True);
        }
    }
}