using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RateLimiter.Abstractions;
using RateLimiter.Models;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class CompositeRuleTests
    {
        private Mock<IRateLimitRule> _mockRule1 = null!;
        private Mock<IRateLimitRule> _mockRule2 = null!;
        private CompositeRule _compositeRule = null!;
        private RequestContext _context = null!;
        
        [SetUp]
        public void Setup()
        {
            _mockRule1 = new Mock<IRateLimitRule>();
            _mockRule2 = new Mock<IRateLimitRule>();
            _compositeRule = new CompositeRule();
            _context = new RequestContext
            {
                ClientToken = "test-token",
                ResourcePath = "/api/resource"
            };
        }
        
        [Test]
        public async Task ValidateAsync_NoRules_ReturnsTrue()
        {
            var result = await _compositeRule.ValidateAsync(_context);
            
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task ValidateAsync_AllRulesPass_ReturnsTrue()
        {
            _mockRule1.Setup(s => s.ValidateAsync(It.IsAny<RequestContext>()))
                .ReturnsAsync(true);
            _mockRule2.Setup(s => s.ValidateAsync(It.IsAny<RequestContext>()))
                .ReturnsAsync(true);
            
            _compositeRule.AddRule(_mockRule1.Object);
            _compositeRule.AddRule(_mockRule2.Object);
            
            var result = await _compositeRule.ValidateAsync(_context);
            
            Assert.That(result, Is.True);
            _mockRule1.Verify(s => s.ValidateAsync(_context), Times.Once);
            _mockRule2.Verify(s => s.ValidateAsync(_context), Times.Once);
        }
        
        [Test]
        public async Task ValidateAsync_FirstRuleFails_ReturnsFalse()
        {
            _mockRule1.Setup(s => s.ValidateAsync(It.IsAny<RequestContext>()))
                .ReturnsAsync(false);
            _mockRule2.Setup(s => s.ValidateAsync(It.IsAny<RequestContext>()))
                .ReturnsAsync(true);
            
            _compositeRule.AddRule(_mockRule1.Object);
            _compositeRule.AddRule(_mockRule2.Object);
            
            var result = await _compositeRule.ValidateAsync(_context);
            
            Assert.That(result, Is.False);
            _mockRule1.Verify(s => s.ValidateAsync(_context), Times.Once);
            _mockRule2.Verify(s => s.ValidateAsync(_context), Times.Never);
        }
        
        [Test]
        public async Task ValidateAsync_SecondRuleFails_ReturnsFalse()
        {
            _mockRule1.Setup(s => s.ValidateAsync(It.IsAny<RequestContext>()))
                .ReturnsAsync(true);
            _mockRule2.Setup(s => s.ValidateAsync(It.IsAny<RequestContext>()))
                .ReturnsAsync(false);
            
            _compositeRule.AddRule(_mockRule1.Object);
            _compositeRule.AddRule(_mockRule2.Object);
            
            var result = await _compositeRule.ValidateAsync(_context);
            
            Assert.That(result, Is.False);
            _mockRule1.Verify(s => s.ValidateAsync(_context), Times.Once);
            _mockRule2.Verify(s => s.ValidateAsync(_context), Times.Once);
        }
    }
}