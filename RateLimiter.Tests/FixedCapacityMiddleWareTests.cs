using System.Threading.Tasks;

using NUnit.Framework;


using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RateLimiter.FixedCapaicityPolicy;
using Moq;
using System;

namespace RateLimiter.Tests;

[TestFixture]
public class FixedCapacityMiddleWareTests
{
    [Test]
    public async Task InvokeAsync_ShouldCallNextMiddleware_WhenCallsAreBelowLimit()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        configuration["RequestLimiterValue"] = "2000"; // Set a valid time span
        var middleware = new FixedCapacityMiddleWare((context) => Task.CompletedTask, cache, configuration);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.AreEqual(200, context.Response.StatusCode);
    }

    [Test]
    public async Task InvokeAsync_ShouldReturn425_WhenCallsExceedLimit()
    {
        // Arrange
        var cacheMock = new Mock<IMemoryCache>();

        cacheMock.Setup(c => c.TryGetValue(It.IsAny<string>(), out It.Ref<object?>.IsAny)).Returns((object key, out object? value) =>
        {
            value = new FixedCapacityValue
            {
                LastRequest = DateTime.Now
            };
            return true;
        });
        var entry = new Mock<ICacheEntry>();
        cacheMock.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(entry.Object);
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        configuration["RequestLimiterValue"] = "10000"; // Set a valid time span
        var middleware = new FixedCapacityMiddleWare((context) => Task.CompletedTask, cacheMock.Object, configuration);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.AreEqual(425, context.Response.StatusCode);
    }
}