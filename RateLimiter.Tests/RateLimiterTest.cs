using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RequestLimiterTests
    {
        private Mock<RequestDelegate> _mockNext;
        private Mock<IRulesFactory> _mockRulesFactory;
        private Mock<IContextHelper> _mockContextHelper;
        private Mock<ICacheHelper> _mockCacheHelper;
        private RequestLimiter _requestLimiter;
        private DefaultHttpContext _httpContext;

        [SetUp]
        public void SetUp()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockRulesFactory = new Mock<IRulesFactory>();
            _mockContextHelper = new Mock<IContextHelper>();
            _mockContextHelper.Setup(ch => ch.GetDto(It.IsAny<HttpContext>())).Returns(new ContextDto { Id = "test-id", Path = "", Region = "US" });
            _mockCacheHelper = new Mock<ICacheHelper>();
            _requestLimiter = new RequestLimiter(_mockNext.Object, _mockRulesFactory.Object, _mockContextHelper.Object, _mockCacheHelper.Object);
            _httpContext = new DefaultHttpContext();
        }

        [Test]
        public async Task InvokeAsync_ShouldCallNextDelegate_WhenRequestIsAllowed()
        {
            // Arrange
            var contextDto = new ContextDto { Id = "test-id" };
            var mockRule = new Mock<IRule>();
            mockRule.Setup(r => r.CheckLimit()).Returns(true);
            _mockContextHelper.Setup(ch => ch.GetDto(_httpContext)).Returns(contextDto);
            _mockRulesFactory.Setup(rf => rf.GetRule(contextDto)).Returns(mockRule.Object);

            // Act
            await _requestLimiter.InvokeAsync(_httpContext);

            // Assert
            _mockNext.Verify(next => next(_httpContext), Times.Once);
            _mockCacheHelper.Verify(ch => ch.AddRequest(contextDto.Id), Times.Once);
        }

        [Test]
        public async Task InvokeAsync_ShouldReturnTooManyRequests_WhenRequestIsNotAllowed()
        {
            // Arrange
            var contextDto = new ContextDto { Id = "test-id" };
            var mockRule = new Mock<IRule>();
            mockRule.Setup(r => r.CheckLimit()).Returns(false);
            _mockContextHelper.Setup(ch => ch.GetDto(_httpContext)).Returns(contextDto);
            _mockRulesFactory.Setup(rf => rf.GetRule(contextDto)).Returns(mockRule.Object);

            // Act
            await _requestLimiter.InvokeAsync(_httpContext);

            // Assert
            Assert.AreEqual(StatusCodes.Status429TooManyRequests, _httpContext.Response.StatusCode);
            _mockNext.Verify(next => next(_httpContext), Times.Never);
            _mockCacheHelper.Verify(ch => ch.AddRequest(It.IsAny<string>()), Times.Never);
        }
    }
}

