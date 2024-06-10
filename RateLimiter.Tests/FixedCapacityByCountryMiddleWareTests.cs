using System.Threading.Tasks;

using NUnit.Framework;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using Microsoft.Extensions.Options;
using RateLimiter.FixedCapacityByCountryPolicy;
using System.Collections.Generic;

namespace RateLimiter.Tests;

[TestFixture]
public class FixedCapacityByCountryMiddleWareTests
{
    [Test]
    public async Task InvokeAsync_ShouldCallNextMiddleware_WhenCallsAreBelowLimit()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var options = Options.Create(new FixedCapacityByCountryMiddleWareOptions
        {
            Items = new List<FixedCapacityByCountryItemMiddleWareOptions>
            {
                    new FixedCapacityByCountryItemMiddleWareOptions
                    {
                        Name = "Default",
                        Amount = 5,
                        TimeSpan = 1000
                    }
                }
        });
        var middleware = new FixedCapacityByCountryMiddleWare((context) => Task.CompletedTask, cache, options);
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
        var cacheMock = new Mock<IMemoryCache>();

        cacheMock.Setup(c => c.TryGetValue(It.IsAny<string>(), out It.Ref<object?>.IsAny)).Returns((object key, out object? value) =>
        {
            value = new FixedCapacityByCountryValue
            {
                Calls = new Dictionary<string, IEnumerable<DateTime>>()
                {
                    {"Default", new List<DateTime> { DateTime.Now }}
                }
            };
            return true;
        });
        var entry = new Mock<ICacheEntry>();
        cacheMock.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(entry.Object);

        var options = Options.Create(new FixedCapacityByCountryMiddleWareOptions
        {
            Items = new List<FixedCapacityByCountryItemMiddleWareOptions>
            {
                    new FixedCapacityByCountryItemMiddleWareOptions
                    {
                        Name = "Default",
                        Amount = 0,
                        TimeSpan = 1000
                    }
            }
        });

        var middleware = new FixedCapacityByCountryMiddleWare((context) => Task.CompletedTask, cacheMock.Object, options);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.AreEqual(429, context.Response.StatusCode);
    }
}