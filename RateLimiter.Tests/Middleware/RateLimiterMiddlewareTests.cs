using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using RateLimiter.Middleware;
using RateLimiter.Services;

namespace RateLimiter.Tests.Middleware;

public class RateLimiterMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldReturn401_WhenTokenIsMissing()
    {
        // Arrange
        var rateLimiterMock = new Mock<IRateLimiterService>();
        var middleware = new RateLimiterMiddleware(async (context) => { await Task.CompletedTask; }, rateLimiterMock.Object);

        var context = new DefaultHttpContext();
        context.Request.Path = "/test/endpoint";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn429_WhenRateLimitIsExceeded()
    {
        // Arrange
        var rateLimiterMock = new Mock<IRateLimiterService>();
        rateLimiterMock
            .Setup(r => r.IsRequestAllowed(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var middleware = new RateLimiterMiddleware(async (context) => { await Task.CompletedTask; }, rateLimiterMock.Object);

        var context = new DefaultHttpContext();
        context.Request.Path = "/test/endpoint";
        context.Request.Headers["X-Client-Token"] = "client-token";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status429TooManyRequests, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallNextMiddleware_WhenRequestIsAllowed()
    {
        // Arrange
        var rateLimiterMock = new Mock<IRateLimiterService>();
        rateLimiterMock
            .Setup(r => r.IsRequestAllowed(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        var nextMiddlewareCalled = false;
        var middleware = new RateLimiterMiddleware(async (context) => { nextMiddlewareCalled = true; await Task.CompletedTask; }, rateLimiterMock.Object);

        var context = new DefaultHttpContext();
        context.Request.Path = "/test/endpoint";
        context.Request.Headers["X-Client-Token"] = "client-token";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextMiddlewareCalled);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }
}