using System;
using System.Collections.Generic;
using NUnit.Framework;
using RateLimiter.Interfaces;
using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.Services;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest {
    [Test]
    public void Example() {
        Assert.That(true, Is.True);
        Assert.That(null, Is.Null);
    }

    [Test]
    [TestCase("identifier", "resource1")]
    [TestCase("identifier", "resource2")]
    public void RequestsPerTimeSpanRuleAllows(string clientIdentifier, string resourceAccessed) {
        RequestCountDatabase database = new RequestCountDatabase();

        database.Add(clientIdentifier, new List<ClientRequest>() {
            new() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-1) },
            new() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-2) },
            new() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-3) },
            new() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-4) },
            new() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-5) },
        });

        var rule = new RequestsPerTimespanRule(database, 10, TimeSpan.FromSeconds(10));

        Assert.True(rule.IsAllowed(clientIdentifier, resourceAccessed));
    }

    [Test]
    [TestCase("identifier1", "resource1")]
    [TestCase("identifier1", "resource2")]
    public void RequestsPerTimeSpanRuleDisallows(string clientIdentifier, string resourceAccessed) {
        RequestCountDatabase database = new RequestCountDatabase();
        database.Add(clientIdentifier, new List<ClientRequest>() {
            new() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-1) },
            new() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-2) },
            new() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-3) },
            new() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-4) },
            new() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-5) },
        });

        var rule = new RequestsPerTimespanRule(database, 3, TimeSpan.FromSeconds(10));

        Assert.False(rule.IsAllowed(clientIdentifier, resourceAccessed));
    }

    [Test]
    [TestCase("identifier2", "resource1")]
    [TestCase("identifier2", "resource2")]
    public void TimespanSinceLastCallRuleAllows(string clientIdentifier, string resourceAccessed) {
        var requestAccessKey = new ClientRequestKey(clientIdentifier, resourceAccessed);
        var database = new LatestRequestDatabase();
        database.Add(requestAccessKey, DateTime.Now.AddSeconds(-1));

        var rule = new TimespanSinceLastCallRule(database, TimeSpan.FromMilliseconds(500));

        Assert.True(rule.IsAllowed(clientIdentifier, resourceAccessed));
    }

    [Test]
    [TestCase("identifier3", "resource1")]
    [TestCase("identifier3", "resource2")]
    public void TimespanSinceLastCallRuleDisallows(string clientIdentifier, string resourceAccessed) {
        var requestAccessKey = new ClientRequestKey(clientIdentifier, resourceAccessed);
        var database = new LatestRequestDatabase();
        database.Add(requestAccessKey, DateTime.Now.AddSeconds(-1));

        var rule = new TimespanSinceLastCallRule(database, TimeSpan.FromSeconds(3));

        Assert.False(rule.IsAllowed(clientIdentifier, resourceAccessed));
    }

    [Test]
    [TestCase("identifier4", "resource1")]
    [TestCase("identifier4", "resource2")]
    public void LimiterWithRequestsPerTimeSpanRuleAllows(string clientIdentifier, string resourceAccessed) {
        RequestCountDatabase database = new RequestCountDatabase();
        var rule = new RequestsPerTimespanRule(database, 10, TimeSpan.FromSeconds(10));
        var limiter = new Limiter();
        limiter.AddRule(rule);

        limiter.LogRequest(clientIdentifier,
            new ClientRequest { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-1) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-2) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-3) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-4) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-5) });


        Assert.True(limiter.isRequestAllowed(clientIdentifier, resourceAccessed));
    }

    [Test]
    [TestCase("identifier5", "resource1")]
    [TestCase("identifier5", "resource2")]
    public void LimiterWithRequestsPerTimeSpanRuleDisallows(string clientIdentifier, string resourceAccessed) {
        RequestCountDatabase database = new RequestCountDatabase();
        var rule = new RequestsPerTimespanRule(database, 3, TimeSpan.FromSeconds(10));
        var limiter = new Limiter();
        limiter.AddRule(rule);

        limiter.LogRequest(clientIdentifier,
            new ClientRequest { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-1) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-2) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-3) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-4) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-5) });


        Assert.False(limiter.isRequestAllowed(clientIdentifier, resourceAccessed));
    }

    [Test]
    [TestCase("identifier6", "resource1")]
    [TestCase("identifier6", "resource2")]
    public void LimiterWithTimespanSinceLastCallRuleAllows(string clientIdentifier, string resourceAccessed) {
        var database = new LatestRequestDatabase();
        var rule = new TimespanSinceLastCallRule(database, TimeSpan.FromMilliseconds(500));
        var limiter = new Limiter();
        limiter.AddRule(rule);

        limiter.LogRequest(clientIdentifier,
            new ClientRequest() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-3) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-2) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-1) });
        


        Assert.True(limiter.isRequestAllowed(clientIdentifier, resourceAccessed));
    }

    [Test]
    [TestCase("identifier7", "resource1")]
    [TestCase("identifier7", "resource2")]
    public void LimiterWithTimespanSinceLastCallRuleDisallows(string clientIdentifier, string resourceAccessed) {
        var database = new LatestRequestDatabase();
        var rule = new TimespanSinceLastCallRule(database, TimeSpan.FromSeconds(3));
        var limiter = new Limiter();
        limiter.AddRule(rule);

        limiter.LogRequest(clientIdentifier,
            new ClientRequest() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-4) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-3) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-2) });
        


        Assert.False(limiter.isRequestAllowed(clientIdentifier, resourceAccessed));
    }

    [Test]
    [TestCase("identifier6", "resource1")]
    [TestCase("identifier6", "resource2")]
    public void LimiterWithBothAllows(string clientIdentifier, string resourceAccessed) {
        var latestRequestDatabase = new LatestRequestDatabase();
        var requestCountDatabase = new RequestCountDatabase();
        var latestRequestRule = new TimespanSinceLastCallRule(latestRequestDatabase, TimeSpan.FromSeconds(0.5));
        var requestCountRule = new RequestsPerTimespanRule(requestCountDatabase, 10, TimeSpan.FromSeconds(10));
        var limiter = new Limiter();
        limiter.AddRule(latestRequestRule);
        limiter.AddRule(requestCountRule);

        limiter.LogRequest(clientIdentifier,
            new ClientRequest() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-3) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-2) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-1) });

        Assert.True(limiter.isRequestAllowed(clientIdentifier, resourceAccessed));
    }

    [Test]
    [TestCase("identifier7", "resource1")]
    [TestCase("identifier7", "resource2")]
    public void LimiterWithBothDisallows(string clientIdentifier, string resourceAccessed) {
        var latestRequestDatabase = new LatestRequestDatabase();
        var requestCountDatabase = new RequestCountDatabase();
        var latestRequestRule = new TimespanSinceLastCallRule(latestRequestDatabase, TimeSpan.FromMilliseconds(500));
        var requestCountRule = new RequestsPerTimespanRule(requestCountDatabase, 2, TimeSpan.FromSeconds(10));
        var limiter = new Limiter();
        limiter.AddRule(latestRequestRule);
        limiter.AddRule(requestCountRule);

        limiter.LogRequest(clientIdentifier,
            new ClientRequest() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-3) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-2) });
        limiter.LogRequest(clientIdentifier,
            new ClientRequest() { ResourceAccessed = resourceAccessed, DateOfAccess = DateTime.Now.AddSeconds(-1) });

        Assert.False(limiter.isRequestAllowed(clientIdentifier, resourceAccessed));
    }

    [Test]
    [TestCase("", "resource1")]
    [TestCase("identifier7", "")]
    public void DisallowsEmptyRequests(string clientIdentifier, string resourceAccessed) {
        var latestRequestDatabase = new LatestRequestDatabase();
        var requestCountDatabase = new RequestCountDatabase();
        var latestRequestRule = new TimespanSinceLastCallRule(latestRequestDatabase, TimeSpan.FromMilliseconds(500));
        var requestCountRule = new RequestsPerTimespanRule(requestCountDatabase, 2, TimeSpan.FromSeconds(10));
        var limiter = new Limiter();
        limiter.AddRule(latestRequestRule);
        limiter.AddRule(requestCountRule);
        
        Assert.False(limiter.isRequestAllowed(clientIdentifier, resourceAccessed));
    }
}