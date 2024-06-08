using System;
using Microsoft.AspNetCore.Mvc;
using RateLimiter.Api;
using Xunit;

namespace RateLimiter.Tests;

public class RateLimiterControllerTests
{
    private readonly RateLimiterApiController _controller;

    public RateLimiterControllerTests()
    {
        _controller = new RateLimiterApiController();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(9)]
    public void GetFixedLimit_AllowsRequest_ReturnsOk(int rateLimit)
    {
        // Arrange
        const string clientId = "testClient1";

        // Act
        IActionResult? result = null;
        for (int i = 0; i < rateLimit; i++) // <=10 calls in a min
        {
            result = _controller.GetFixedLimit(clientId);
        }

        // Assert
        Assert.IsType<OkResult>(result);
        Assert.Equal(200, ((OkResult)result).StatusCode);
    }

    [Fact]
    public void GetFixedLimit_BlocksRequest_ReturnsTooManyRequests()
    {
        // Arrange
        const string clientId = "testClient1";

        // Act
        for (int i = 0; i < 10; i++)
        {
            _controller.GetFixedLimit(clientId);
        }
        var result = _controller.GetFixedLimit(clientId); // 11th call in a min

        // Assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal(429, ((ObjectResult)result).StatusCode);
        Assert.Equal(RateLimiterApiController.TooManyRequestsMessage, ((ObjectResult)result).Value);
    }

    [Fact]
    public void GetSlidingLimit_AllowsRequest_ReturnsOk()
    {
        // Arrange
        const string clientId = "testClient2";

        // Act
        _controller.GetSlidingLimit(clientId);

        var endTime = DateTime.UtcNow.AddSeconds(5);  // delay 5 sec
        while (DateTime.UtcNow < endTime) { }

        var result = _controller.GetSlidingLimit(clientId);

        // Assert
        Assert.IsType<OkResult>(result);
        Assert.Equal(200, ((OkResult)result).StatusCode);
    }

    [Fact]
    public void GetSlidingLimit_BlocksRequest_ReturnsTooManyRequests()
    {
        // Arrange
        const string clientId = "testClient2";

        // Act
        IActionResult? result = null;
        for (int i = 0; i < 2; i++) // 2 calls in 10 sec
        {
            result = _controller.GetSlidingLimit(clientId);
        }

        // Assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal(429, ((ObjectResult)result).StatusCode);
        Assert.Equal(RateLimiterApiController.TooManyRequestsMessage, ((ObjectResult)result).Value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(9)]
    public void GetRegionBasedLimit_US_AllowsRequest_ReturnsOk(int rateLimit)
    {
        // Arrange
        const string clientId = "US_testClient3";

        // Act
        IActionResult? result = null;
        for (int i = 0; i < rateLimit; i++) // <=10 calls in a min
        {
            result = _controller.GetRegionBasedLimit(clientId);
        }

        // Assert
        Assert.IsType<OkResult>(result);
        Assert.Equal(200, ((OkResult)result).StatusCode);
    }

    [Fact]
    public void GetRegionBasedLimit_US_BlocksRequest_ReturnsTooManyRequests()
    {
        // Arrange
        const string clientId = "US_testClient3";

        // Act
        for (int i = 0; i < 20; i++)
        {
            _controller.GetRegionBasedLimit(clientId);
        }

        var result = _controller.GetRegionBasedLimit(clientId); // 21th call in a min

        // Assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal(429, ((ObjectResult)result).StatusCode);
        Assert.Equal(RateLimiterApiController.TooManyRequestsMessage, ((ObjectResult)result).Value);
    }

    [Fact]
    public void GetRegionBasedLimit_EU_AllowsRequest_ReturnsOk()
    {
        // Arrange
        const string clientId = "EU_testClient3";

        // Act
        _controller.GetRegionBasedLimit(clientId);

        var endTime = DateTime.UtcNow.AddSeconds(5);  // delay 5 sec
        while (DateTime.UtcNow < endTime) { }

        var result = _controller.GetRegionBasedLimit(clientId);

        // Assert
        Assert.IsType<OkResult>(result);
        Assert.Equal(200, ((OkResult)result).StatusCode);
    }

    [Fact]
    public void GetRegionBasedLimit_EU_BlocksRequest_ReturnsTooManyRequests()
    {
        // Arrange
        const string clientId = "EU_testClient3";

        // Act
        IActionResult? result = null;
        for (int i = 0; i < 2; i++) // 2 calls in 10 sec
        {
            result = _controller.GetRegionBasedLimit(clientId);
        }

        // Assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal(429, ((ObjectResult)result).StatusCode);
        Assert.Equal(RateLimiterApiController.TooManyRequestsMessage, ((ObjectResult)result).Value);
    }
}