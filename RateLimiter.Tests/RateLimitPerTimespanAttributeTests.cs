using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using RateLimiter.Attributes;
using RateLimiter.Tests.Helpers;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimitPerTimespanAttributeTests
{
    [Test]
    [TestCase(5)]
    public void RateLimitPerTimespan_AllowsRequest_WithinLimit(int maxRequests)
    {
        // Arrange
        var filter = new RateLimitPerTimespanAttribute(maxRequests, 60);
        var context = ActionFilterHelper.CreateActionExecutingContext("token", "ActionName");

        for (var i = 0; i < maxRequests; i++)
        {
            // Act
            filter.OnActionExecuting(context);
        }

        // Assert
        Assert.IsNull(context.Result);
    }

    [Test]
    public void RateLimitPerTimespan_BlocksRequest_ExceedsLimit()
    {
        // Arrange
        var filter = new RateLimitPerTimespanAttribute(1, 60);
        var context = ActionFilterHelper.CreateActionExecutingContext("token", "ActionName");

        // Act
        filter.OnActionExecuting(context);
        filter.OnActionExecuting(context); // Should be blocked

        // Assert
        Assert.IsInstanceOf<ContentResult>(context.Result);
        Assert.AreEqual(429, ((ContentResult)context.Result).StatusCode);
    }
}