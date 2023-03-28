using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using RateLimiter.Rules;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
    [Test]
    public void Example()
    {
        var clients = Initializer.GetClients();
        var client = clients.FirstOrDefault();

        RequestsBucket.Add(Initializer.CreateRequest(client?.Identifier));
        RequestsBucket.Add(Initializer.CreateRequest(client?.Identifier));
        RequestsBucket.Add(Initializer.CreateRequest(client?.Identifier));
        RequestsBucket.Add(Initializer.CreateRequest(client?.Identifier));

        Thread.Sleep(TimeSpan.FromSeconds(3));

        var request = Initializer.CreateRequest(client?.Identifier);

        var result = client?.RateLimitRules?.Handle(request);

        Assert.IsFalse(result);

        RequestsBucket.Clear();
    }

    [Test]
    public void Example2()
    {
        var clients = Initializer.GetClients();
        var client = clients.LastOrDefault();

        RequestsBucket.Add(Initializer.CreateRequest(client?.Identifier));
        RequestsBucket.Add(Initializer.CreateRequest(client?.Identifier));
        RequestsBucket.Add(Initializer.CreateRequest(client?.Identifier));
        RequestsBucket.Add(Initializer.CreateRequest(client?.Identifier));

        Thread.Sleep(TimeSpan.FromSeconds(3));

        var request = Initializer.CreateRequest(client?.Identifier);

        var result = client?.RateLimitRules?.Handle(request);

        Assert.IsFalse(result);
        
        RequestsBucket.Clear();
    }

    [Test]
    public void Example3()
    {
        var clients = Initializer.GetClients();
        var client = clients.FirstOrDefault();

        RequestsBucket.Add(Initializer.CreateRequest(client?.Identifier));
        RequestsBucket.Add(Initializer.CreateRequest(client?.Identifier));
        RequestsBucket.Add(Initializer.CreateRequest(client?.Identifier));

        Thread.Sleep(TimeSpan.FromSeconds(3));

        var request = Initializer.CreateRequest(client?.Identifier);

        var result = client?.RateLimitRules?.Handle(request);

        Assert.IsTrue(result);
        
        RequestsBucket.Clear();
    }
}