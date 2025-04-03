using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RateLimiter.Abstractions;
using RateLimiter.Models;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class RegionCompositeRuleTests
    {
        private Mock<IRateLimitRule> _mockDefaultRule = null!;
        private Mock<IRateLimitRule> _mockUsRule = null!;
        private Mock<IRateLimitRule> _mockEuRule = null!;
        private RegionCompositeRule _regionRule = null!;
        
        [SetUp]
        public void Setup()
        {
            _mockDefaultRule = new Mock<IRateLimitRule>();
            _mockUsRule = new Mock<IRateLimitRule>();
            _mockEuRule = new Mock<IRateLimitRule>();
            
            _regionRule = new RegionCompositeRule(_mockDefaultRule.Object);
            _regionRule.AddRegionRule("US", _mockUsRule.Object);
            _regionRule.AddRegionRule("EU", _mockEuRule.Object);
        }
        
        [Test]
        public async Task ValidateAsync_WithUsRegion_UsesUsRule()
        {
            var context = new RequestContext
            {
                ClientToken = "test-token",
                ResourcePath = "/api/resource",
                Region = "US"
            };
            
            _mockUsRule.Setup(s => s.ValidateAsync(It.IsAny<RequestContext>()))
                .ReturnsAsync(true);
            
            var result = await _regionRule.ValidateAsync(context);
            
            Assert.That(result, Is.True);
            _mockUsRule.Verify(s => s.ValidateAsync(context), Times.Once);
            _mockEuRule.Verify(s => s.ValidateAsync(It.IsAny<RequestContext>()), Times.Never);
            _mockDefaultRule.Verify(s => s.ValidateAsync(It.IsAny<RequestContext>()), Times.Never);
        }
        
        [Test]
        public async Task ValidateAsync_WithEuRegion_UsesEuRule()
        {
            var context = new RequestContext
            {
                ClientToken = "test-token",
                ResourcePath = "/api/resource",
                Region = "EU"
            };
            
            _mockEuRule.Setup(s => s.ValidateAsync(It.IsAny<RequestContext>()))
                .ReturnsAsync(true);
            
            var result = await _regionRule.ValidateAsync(context);
            
            Assert.That(result, Is.True);
            _mockEuRule.Verify(s => s.ValidateAsync(context), Times.Once);
            _mockUsRule.Verify(s => s.ValidateAsync(It.IsAny<RequestContext>()), Times.Never);
            _mockDefaultRule.Verify(s => s.ValidateAsync(It.IsAny<RequestContext>()), Times.Never);
        }
        
        [Test]
        public async Task ValidateAsync_WithUnknownRegion_UsesDefaultRule()
        {
            var context = new RequestContext
            {
                ClientToken = "test-token",
                ResourcePath = "/api/resource",
                Region = "UNKNOWN"
            };
            
            _mockDefaultRule.Setup(s => s.ValidateAsync(It.IsAny<RequestContext>()))
                .ReturnsAsync(true);
            
            var result = await _regionRule.ValidateAsync(context);
            
            Assert.That(result, Is.True);
            _mockDefaultRule.Verify(s => s.ValidateAsync(context), Times.Once);
            _mockUsRule.Verify(s => s.ValidateAsync(It.IsAny<RequestContext>()), Times.Never);
            _mockEuRule.Verify(s => s.ValidateAsync(It.IsAny<RequestContext>()), Times.Never);
        }
        
        [Test]
        public async Task ValidateAsync_WithEmptyRegion_UsesDefaultRule()
        {
            var context = new RequestContext
            {
                ClientToken = "test-token",
                ResourcePath = "/api/resource",
                Region = string.Empty
            };
            
            _mockDefaultRule.Setup(s => s.ValidateAsync(It.IsAny<RequestContext>()))
                .ReturnsAsync(true);
            
            var result = await _regionRule.ValidateAsync(context);
            
            Assert.That(result, Is.True);
            _mockDefaultRule.Verify(s => s.ValidateAsync(context), Times.Once);
            _mockUsRule.Verify(s => s.ValidateAsync(It.IsAny<RequestContext>()), Times.Never);
            _mockEuRule.Verify(s => s.ValidateAsync(It.IsAny<RequestContext>()), Times.Never);
        }
    }
}