using Moq;
using NUnit.Framework;
using RateLimiter.Resources;
using RateLimiter.Handlers;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{

    [SetUp]
    public void Init()
    {

    }

    [TearDown]
    public void Cleanup()
    { /* ... */ }
    
    [Test]
    public void ProxyReturnsSuccessWithNoHandler()
    {
        BucketLimiterManager manager = new BucketLimiterManager();
        string request = "GetOrders";
        string clientID = Guid.NewGuid().ToString();
        var orderServiceMock = CreateOrderServiceMock(request, clientID);

        manager.RegisterResource(orderServiceMock.Object);

        var actual = manager.GetResource(orderServiceMock.Object, request, clientID);
        var expected = orderServiceMock.Object.GetResource(request, clientID);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TokenBucketHandlerSuccess()
    {
        BucketLimiterManager manager = new BucketLimiterManager();
        string request = "GetOrders";
        string clientID = Guid.NewGuid().ToString();

        var orderServiceMock = CreateOrderServiceMock(request, clientID);
        manager.RegisterResource(orderServiceMock.Object);

        int capacity = 4;
        int refreshRate = 100;
        var settings = new RateLimiterSettings(capacity, refreshRate);
        List<ILimiterHandler> handlers = new List<ILimiterHandler>();
        handlers.Add(new TokenBucketHandler(settings));
        manager.SetHandlers(orderServiceMock.Object, handlers);
        var expected = orderServiceMock.Object.GetResource(request, clientID);

        var actual = manager.GetResource(orderServiceMock.Object, request, clientID);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TokenBucketLimiterHandlerFail()
    {
        BucketLimiterManager manager = new BucketLimiterManager();
        string request = "GetOrders";
        string clientID = Guid.NewGuid().ToString();

        var orderServiceMock = CreateOrderServiceMock(request, clientID);
        manager.RegisterResource(orderServiceMock.Object);

        int capacity = 4;
        int refreshRate = 1000;
        var settings = new RateLimiterSettings(capacity, refreshRate);
        List<ILimiterHandler> handlers = new List<ILimiterHandler>();
        handlers.Add(new TokenBucketHandler(settings));
        manager.SetHandlers(orderServiceMock.Object, handlers);
        var expected = Constants.LIMIT_EXCEEDED;

        int counter = capacity+1;
        string actual = string.Empty;
        while (counter-- > 0)
             actual = manager.GetResource(orderServiceMock.Object, request, clientID);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void FixedWindowLimiterHandlerSuccess()
    {
        BucketLimiterManager manager = new BucketLimiterManager();
        string request = "GetOrders";
        string clientID = Guid.NewGuid().ToString();

        var orderServiceMock = CreateOrderServiceMock(request, clientID);
        manager.RegisterResource(orderServiceMock.Object);

        int capacity = 4;
        int refreshRate = 1;
        var settings = new RateLimiterSettings(capacity, refreshRate);
        List<ILimiterHandler> handlers = new List<ILimiterHandler>();
        handlers.Add(new FixedWindowHandler(settings));
        manager.SetHandlers(orderServiceMock.Object, handlers);
        var expected = orderServiceMock.Object.GetResource(request, clientID);

        int counter = capacity;
        string actual = string.Empty;
        while (counter-- > 0)
            actual = manager.GetResource(orderServiceMock.Object, request, clientID);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void MixedHandlersSuccess()
    {
        BucketLimiterManager manager = new BucketLimiterManager();
        string request = "GetOrders";
        string clientID = Guid.NewGuid().ToString();

        var orderServiceMock = CreateOrderServiceMock(request, clientID);
        manager.RegisterResource(orderServiceMock.Object);

        int capacity = 4;
        int refreshRate = 1000;
        var settings = new RateLimiterSettings(capacity, refreshRate);

        int capacity2 = 2;
        int refreshRate2 = 1000;
        var settings2 = new RateLimiterSettings(capacity2, refreshRate2);

        List<ILimiterHandler> handlers = new List<ILimiterHandler>();
        handlers.Add(new TokenBucketHandler(settings));
        handlers.Add(new FixedWindowHandler(settings2));
        manager.SetHandlers(orderServiceMock.Object, handlers);
        var expected = orderServiceMock.Object.GetResource(request, clientID);

        int counter = Math.Min(capacity, capacity2);
        string actual = string.Empty;
        while (counter-- > 0)
            actual = manager.GetResource(orderServiceMock.Object, request, clientID);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void MixedHandlersFail()
    {
        BucketLimiterManager manager = new BucketLimiterManager();
        string request = "GetOrders";
        string clientID = Guid.NewGuid().ToString();

        var orderServiceMock = CreateOrderServiceMock(request, clientID);
        manager.RegisterResource(orderServiceMock.Object);

        int capacity = 4;
        int refreshRate = 1;
        var settings = new RateLimiterSettings(capacity, refreshRate);

        int capacity2 = 2;
        int refreshRate2 = 1;
        var settings2 = new RateLimiterSettings(capacity2, refreshRate2);

        List<ILimiterHandler> handlers = new List<ILimiterHandler>();
        handlers.Add(new TokenBucketHandler(settings));
        handlers.Add(new FixedWindowHandler(settings2));
        manager.SetHandlers(orderServiceMock.Object, handlers);
        var expected = Constants.LIMIT_EXCEEDED;

        int counter = Math.Min(capacity, capacity2) + 1; 
        string actual = string.Empty;
        while (counter-- >= 0)
            actual = manager.GetResource(orderServiceMock.Object, request, clientID);
        Assert.AreEqual(expected, actual);
    }

    Mock<IResource> CreateOrderServiceMock(string request, string clientID)
    {
        string response = String.Format("You've got orders {0} for client {1}.", request, clientID);
        var orderService = new Mock<IResource>();
        orderService.Setup(x => x.GetResource(request, clientID.ToString())).Returns(response);
        return orderService;
    }
}