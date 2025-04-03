using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RateLimiter.Abstractions;
using RateLimiter.Configuration;
using RateLimiter.Http;
using RateLimiter.Models;

namespace RateLimiter.Tests.Http
{
    [TestFixture]
    public class RateLimiterMiddlewareTests
    {
        private Mock<RequestDelegate> _mockNext = null!;
        private Mock<IRequestContextFactory> _mockContextFactory = null!;
        private Mock<IOptionsMonitor<RateLimitOptions>> _mockOptionsMonitor = null!;
        private Mock<IRateLimitRule> _mockRule = null!;
        private RateLimiterMiddleware _middleware = null!;
        private DefaultHttpContext _httpContext = null!;
        private RequestContext _requestContext = null!;
        private RateLimitOptions _options = null!;
        
        [SetUp]
        public void Setup()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockContextFactory = new Mock<IRequestContextFactory>();
            _mockOptionsMonitor = new Mock<IOptionsMonitor<RateLimitOptions>>();
            _mockRule = new Mock<IRateLimitRule>();
            
            _middleware = new RateLimiterMiddleware(_mockNext.Object, _mockContextFactory.Object);
            
            _httpContext = new DefaultHttpContext();
            _httpContext.Request.Path = "/api/resource";
            
            _requestContext = new RequestContext
            {
                ClientToken = "test-token",
                ResourcePath = "/api/resource"
            };
            
            _options = new RateLimitOptions();
            _options.DefaultRules.Add(_mockRule.Object);
            
            _mockContextFactory.Setup(f => f.Create(It.IsAny<HttpContext>()))
                .Returns(_requestContext);
            
            _mockOptionsMonitor.Setup(o => o.CurrentValue)
                .Returns(_options);
        }
        
        [Test]
        public async Task InvokeAsync_WhenRateLimitAllowed_CallsNextMiddleware()
        {
            _mockRule.Setup(s => s.ValidateAsync(It.IsAny<RequestContext>()))
                .ReturnsAsync(true);
            
            await _middleware.InvokeAsync(_httpContext, _mockOptionsMonitor.Object);
            
            _mockNext.Verify(n => n(_httpContext), Times.Once);
            Assert.That(_httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }
        
        [Test]
        public async Task InvokeAsync_WhenRateLimitExceeded_Returns429()
        {
            _mockRule.Setup(s => s.ValidateAsync(It.IsAny<RequestContext>()))
                .ReturnsAsync(false);
            
            await _middleware.InvokeAsync(_httpContext, _mockOptionsMonitor.Object);
            
            _mockNext.Verify(n => n(It.IsAny<HttpContext>()), Times.Never);
            Assert.That(_httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status429TooManyRequests));
            Assert.That(_httpContext.Response.Headers.ContainsKey("Retry-After"), Is.True);
        }
        
        [Test]
        public async Task InvokeAsync_WithMultipleRules_ChecksAllRules()
        {
            var mockRule2 = new Mock<IRateLimitRule>();
            _options.DefaultRules.Add(mockRule2.Object);
            
            _mockRule.Setup(s => s.ValidateAsync(It.IsAny<RequestContext>()))
                .ReturnsAsync(true);
            mockRule2.Setup(s => s.ValidateAsync(It.IsAny<RequestContext>()))
                .ReturnsAsync(true);
            
            await _middleware.InvokeAsync(_httpContext, _mockOptionsMonitor.Object);
            
            _mockRule.Verify(s => s.ValidateAsync(_requestContext), Times.Once);
            mockRule2.Verify(s => s.ValidateAsync(_requestContext), Times.Once);
            _mockNext.Verify(n => n(_httpContext), Times.Once);
        }
        
        [Test]
        public async Task InvokeAsync_FirstRuleFails_DoesNotCheckSecondRule()
        {
            var mockRule2 = new Mock<IRateLimitRule>();
            _options.DefaultRules.Add(mockRule2.Object);
            
            _mockRule.Setup(s => s.ValidateAsync(It.IsAny<RequestContext>()))
                .ReturnsAsync(false);
            
            await _middleware.InvokeAsync(_httpContext, _mockOptionsMonitor.Object);
            
            _mockRule.Verify(s => s.ValidateAsync(_requestContext), Times.Once);
            mockRule2.Verify(s => s.ValidateAsync(It.IsAny<RequestContext>()), Times.Never);
            _mockNext.Verify(n => n(It.IsAny<HttpContext>()), Times.Never);
        }
    }
}