using NUnit.Framework;
using System;
using System.Threading;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
    [Test]
    public void Example()
    {
        Assert.That(true, Is.True);
    }

    [Test]
    public void TimeIntervalBetweenRequestsRule_ShouldAllowFirstRequest()
    {
        // Arrange
        var requestTracker = new RequestTracker();
        var rule = new TimeIntervalBetweenRequestsRule(requestTracker, TimeSpan.FromSeconds(1));
        
        // Act
        bool isAllowed = rule.IsRequestAllowed("token1", "resource1");
        
        // Assert
        Assert.That(isAllowed, Is.True);
    }

    [Test]
    public void TimeIntervalBetweenRequestsRule_ShouldBlockRequestsWithinInterval()
    {
        // Arrange
        var requestTracker = new RequestTracker();
        var rule = new TimeIntervalBetweenRequestsRule(requestTracker, TimeSpan.FromSeconds(1));
        
        // Act
        requestTracker.RecordRequest("token1", "resource1");
        bool isAllowed = rule.IsRequestAllowed("token1", "resource1");
        
        // Assert
        Assert.That(isAllowed, Is.False);
    }

    [Test]
    public void TimeIntervalBetweenRequestsRule_ShouldAllowRequestsAfterInterval()
    {
        // Arrange
        var requestTracker = new RequestTracker();
        var rule = new TimeIntervalBetweenRequestsRule(requestTracker, TimeSpan.FromMilliseconds(50));
        
        // Act
        requestTracker.RecordRequest("token1", "resource1");
        Thread.Sleep(100); // Wait longer than the interval
        bool isAllowed = rule.IsRequestAllowed("token1", "resource1");
        
        // Assert
        Assert.That(isAllowed, Is.True);
    }

    [Test]
    public void RequestsPerTimespanRule_ShouldAllowRequestsWithinLimit()
    {
        // Arrange
        var requestTracker = new RequestTracker();
        var rule = new RequestsPerTimespanRule(requestTracker, 3, TimeSpan.FromSeconds(10));
        
        // Act
        requestTracker.RecordRequest("token1", "resource1");
        requestTracker.RecordRequest("token1", "resource1");
        bool isAllowed = rule.IsRequestAllowed("token1", "resource1");
        
        // Assert
        Assert.That(isAllowed, Is.True);
    }

    [Test]
    public void RequestsPerTimespanRule_ShouldBlockRequestsOverLimit()
    {
        // Arrange
        var requestTracker = new RequestTracker();
        var rule = new RequestsPerTimespanRule(requestTracker, 2, TimeSpan.FromSeconds(10));
        
        // Act
        requestTracker.RecordRequest("token1", "resource1");
        requestTracker.RecordRequest("token1", "resource1");
        bool isAllowed = rule.IsRequestAllowed("token1", "resource1");
        
        // Assert
        Assert.That(isAllowed, Is.False);
    }

    [Test]
    public void CompositeRule_And_ShouldRequireAllRulesToPass()
    {
        // Arrange
        var requestTracker = new RequestTracker();
        var rule1 = new RequestsPerTimespanRule(requestTracker, 3, TimeSpan.FromSeconds(10));
        var rule2 = new TimeIntervalBetweenRequestsRule(requestTracker, TimeSpan.FromMilliseconds(50));
        
        var compositeRule = new CompositeRule(CompositeRule.LogicalOperator.And);
        compositeRule.AddRule(rule1);
        compositeRule.AddRule(rule2);
        
        // Act - First request should pass both rules
        bool firstRequest = compositeRule.IsRequestAllowed("token1", "resource1");
        
        // Record the request
        requestTracker.RecordRequest("token1", "resource1");
        
        // Second request should fail the interval rule but pass the count rule
        bool secondRequest = compositeRule.IsRequestAllowed("token1", "resource1");
        
        // Wait for the interval to pass
        Thread.Sleep(100);
        
        // Third request should now pass both rules again
        bool thirdRequest = compositeRule.IsRequestAllowed("token1", "resource1");
        
        // Assert
        Assert.That(firstRequest, Is.True);
        Assert.That(secondRequest, Is.False);
        Assert.That(thirdRequest, Is.True);
    }

    [Test]
    public void CompositeRule_Or_ShouldRequireAnyRuleToPass()
    {
        // Arrange
        var requestTracker = new RequestTracker();
        var rule1 = new RequestsPerTimespanRule(requestTracker, 1, TimeSpan.FromSeconds(10));
        var rule2 = new TimeIntervalBetweenRequestsRule(requestTracker, TimeSpan.FromMilliseconds(50));
        
        var compositeRule = new CompositeRule(CompositeRule.LogicalOperator.Or);
        compositeRule.AddRule(rule1);
        compositeRule.AddRule(rule2);
        
        // Act - First request should pass both rules
        bool firstRequest = compositeRule.IsRequestAllowed("token1", "resource1");
        
        // Record the request
        requestTracker.RecordRequest("token1", "resource1");
        
        // Second request should fail the count rule but pass the interval rule after waiting
        Thread.Sleep(100);
        bool secondRequest = compositeRule.IsRequestAllowed("token1", "resource1");
        
        // Record the second request
        requestTracker.RecordRequest("token1", "resource1");
        
        // Third request should fail both rules (over count limit and within interval)
        bool thirdRequest = compositeRule.IsRequestAllowed("token1", "resource1");
        
        // Assert
        Assert.That(firstRequest, Is.True);
        Assert.That(secondRequest, Is.True);
        Assert.That(thirdRequest, Is.False);
    }

    [Test]
    public void RegionalRule_ShouldApplyDifferentRulesBasedOnRegion()
    {
        // Arrange
        var requestTracker = new RequestTracker();
        
        // Strict rule for US region - only 1 request allowed
        var usRule = new RequestsPerTimespanRule(requestTracker, 1, TimeSpan.FromSeconds(10));
        
        // Lenient rule for EU region - 3 requests allowed
        var euRule = new RequestsPerTimespanRule(requestTracker, 3, TimeSpan.FromSeconds(10));
        
        // Function to resolve region from token
        Func<string, Region> regionResolver = token => 
        {
            if (token.StartsWith("us-"))
                return Region.US;
            else if (token.StartsWith("eu-"))
                return Region.EU;
            else
                return Region.Other;
        };
        
        var regionalRule = new RegionalRule(regionResolver);
        regionalRule.SetRuleForRegion(Region.US, usRule);
        regionalRule.SetRuleForRegion(Region.EU, euRule);
        
        // Act & Assert
        
        // US token - first request allowed
        Assert.That(regionalRule.IsRequestAllowed("us-token", "resource1"), Is.True);
        requestTracker.RecordRequest("us-token", "resource1");
        
        // US token - second request blocked (over limit)
        Assert.That(regionalRule.IsRequestAllowed("us-token", "resource1"), Is.False);
        
        // EU token - first request allowed
        Assert.That(regionalRule.IsRequestAllowed("eu-token", "resource1"), Is.True);
        requestTracker.RecordRequest("eu-token", "resource1");
        
        // EU token - second request allowed
        Assert.That(regionalRule.IsRequestAllowed("eu-token", "resource1"), Is.True);
        requestTracker.RecordRequest("eu-token", "resource1");
        
        // EU token - third request allowed
        Assert.That(regionalRule.IsRequestAllowed("eu-token", "resource1"), Is.True);
        requestTracker.RecordRequest("eu-token", "resource1");
        
        // EU token - fourth request blocked (over limit)
        Assert.That(regionalRule.IsRequestAllowed("eu-token", "resource1"), Is.False);
        
        // Other region - no rule set, so allowed
        Assert.That(regionalRule.IsRequestAllowed("other-token", "resource1"), Is.True);
    }

    [Test]
    public void RateLimiter_ShouldApplyRulesForResources()
    {
        // Arrange
        var rateLimiter = new RateLimiter();
        var requestTracker = rateLimiter.GetRequestTracker();
        
        // Create rules
        var rule1 = new RequestsPerTimespanRule(requestTracker, 2, TimeSpan.FromSeconds(10));
        var rule2 = new TimeIntervalBetweenRequestsRule(requestTracker, TimeSpan.FromMilliseconds(50));
        
        // Set rules for different resources
        rateLimiter.SetRuleForResource("resource1", rule1);
        rateLimiter.SetRuleForResource("resource2", rule2);
        
        // Act & Assert
        
        // Resource 1 - first request allowed
        Assert.That(rateLimiter.IsRequestAllowed("token1", "resource1"), Is.True);
        requestTracker.RecordRequest("token1", "resource1");
        
        // Resource 1 - second request allowed
        Assert.That(rateLimiter.IsRequestAllowed("token1", "resource1"), Is.True);
        requestTracker.RecordRequest("token1", "resource1");
        
        // Resource 1 - third request blocked (over limit)
        Assert.That(rateLimiter.IsRequestAllowed("token1", "resource1"), Is.False);
        
        // Resource 2 - first request allowed
        Assert.That(rateLimiter.IsRequestAllowed("token1", "resource2"), Is.True);
        requestTracker.RecordRequest("token1", "resource2");
        
        // Resource 2 - second request blocked (within interval)
        Assert.That(rateLimiter.IsRequestAllowed("token1", "resource2"), Is.False);
        
        // Wait for interval to pass
        Thread.Sleep(100);
        
        // Resource 2 - third request allowed (after interval)
        Assert.That(rateLimiter.IsRequestAllowed("token1", "resource2"), Is.True);
        
        // Resource 3 - no rule set, so allowed
        Assert.That(rateLimiter.IsRequestAllowed("token1", "resource3"), Is.True);
    }
}
