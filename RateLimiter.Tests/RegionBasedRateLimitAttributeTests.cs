using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using RateLimiter.Attributes;
using RateLimiter.Tests.Helpers;

namespace RateLimiter.Tests;

[TestFixture]
public class RegionBasedRateLimitAttributeTests
{
    [Test]
    [TestCase(5)]
    public void RegionBasedRateLimit_AllowsUSRequest_WithinLimit(int maxRequests)
    {
        // Arrange
        var filter = new RegionBasedRateLimitAttribute(maxRequests, 60, 10);
        var context = ActionFilterHelper.CreateActionExecutingContext("US-token", "ActionName");

        for (var i = 0; i < maxRequests; i++)
        {
            // Act
            filter.OnActionExecuting(context);
        }

        // Assert
        Assert.IsNull(context.Result);
    }

    [Test]
    public void RegionBasedRateLimit_AllowsUKRequest_AfterInterval()
    {
        // Arrange
        var filter = new RegionBasedRateLimitAttribute(5, 60, 1);
        var context = ActionFilterHelper.CreateActionExecutingContext("UK-token", "ActionName");

        // Act
        filter.OnActionExecuting(context);

        // Wait for the interval to pass
        System.Threading.Thread.Sleep(1001);

        // Act
        filter.OnActionExecuting(context);

        // Assert
        Assert.IsNull(context.Result);
    }

    [Test]
    public void RegionBasedRateLimit_BlocksUSRequest_ExceedsLimit()
    {
        // Arrange
        var filter = new RegionBasedRateLimitAttribute(1, 60, 10);
        var context = ActionFilterHelper.CreateActionExecutingContext("US-token", "ActionName");

        // Act
        filter.OnActionExecuting(context);
        filter.OnActionExecuting(context); // Exceed the limit

        // Assert
        Assert.IsInstanceOf<ContentResult>(context.Result);
        Assert.AreEqual(429, ((ContentResult)context.Result).StatusCode);
    }

    [Test]
    public void RegionBasedRateLimit_BlocksUKRequest_BeforeInterval()
    {
        // Arrange
        var filter = new RegionBasedRateLimitAttribute(5, 60, 2);
        var context = ActionFilterHelper.CreateActionExecutingContext("UK-token", "ActionName");

        // Act
        filter.OnActionExecuting(context);
        filter.OnActionExecuting(context); // Should be blocked

        // Assert
        Assert.IsInstanceOf<ContentResult>(context.Result);
        Assert.AreEqual(429, ((ContentResult)context.Result).StatusCode);
    }
}