using Microsoft.Extensions.Options;
using System.Threading.Tasks;

using NUnit.Framework;

using RateLimiter.RequestLimiterPolicy;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;

namespace RateLimiter.Tests;

[TestFixture]
public class RequestLimiterMiddleWareTests
{
    [Test]
    public async Task InvokeAsync_ShouldCallNextMiddleware_WhenCallsAreBelowLimit()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var options = Options.Create(new RequestLimiterMiddleWareOptions { Amount = 5, TimeSpan = 1000 });
        var middleware = new RequestLimiterMiddleWare((context) => Task.CompletedTask, cache, options);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.AreEqual(200, context.Response.StatusCode);
    }

    [Test]
    public async Task InvokeAsync_ShouldReturn429_WhenCallsExceedLimit()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var options = Options.Create(new RequestLimiterMiddleWareOptions { Amount = 0, TimeSpan = 1000 });
        var middleware = new RequestLimiterMiddleWare((context) => Task.CompletedTask, cache, options);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.AreEqual(429, context.Response.StatusCode);
    }
}