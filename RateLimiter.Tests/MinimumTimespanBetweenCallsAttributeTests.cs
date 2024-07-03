using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using RateLimiter.Attributes;
using RateLimiter.Tests.Helpers;

namespace RateLimiter.Tests;

[TestFixture]
public class MinimumTimespanBetweenCallsAttributeTests
{
    [Test]
    public void MinimumTimespanBetweenCalls_AllowsRequest_AfterInterval()
    {
        // Arrange
        var filter = new MinimumTimespanBetweenCallsAttribute(5);
        var context = ActionFilterHelper.CreateActionExecutingContext("token", "ActionName");

        // Act
        filter.OnActionExecuting(context);
        System.Threading.Thread.Sleep(5001); // Wait for the interval to pass
        filter.OnActionExecuting(context);

        // Assert
        Assert.IsNull(context.Result);
    }

    [Test]
    public void MinimumTimespanBetweenCalls_BlocksRequest_BeforeInterval()
    {
        // Arrange
        var filter = new MinimumTimespanBetweenCallsAttribute(5);
        var context = ActionFilterHelper.CreateActionExecutingContext("token", "ActionName");

        // Act
        filter.OnActionExecuting(context);
        filter.OnActionExecuting(context); // Should be blocked

        // Assert
        Assert.IsInstanceOf<ContentResult>(context.Result);
        Assert.AreEqual(429, ((ContentResult)context.Result).StatusCode);
    }
}