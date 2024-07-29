using System;
using System.Threading;
using NUnit.Framework;
using RateLimiter.Rules;
using RateLimiter.Storage;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
	[Test]
	public void TimePassedRule_ShouldFailWhenViolated()
	{
		var token = "123";
		var timePassedRule = new TimePassed(1000);
		var resource = Guid.NewGuid();
		var statisticsRepository = new AccessStatisticsStaticRepository();
		var accessService = new AccessService(resource, new IRule[] { timePassedRule }, statisticsRepository);

		Assert.That(accessService.HasAccess(token), Is.True);
		Thread.Sleep(500);
		Assert.That(accessService.HasAccess(token), Is.False);
		Thread.Sleep(500);
		Assert.That(accessService.HasAccess(token), Is.True);
	}
    
	[Test]
	public void RequestsPerTimespan_ShouldFailWhenViolated()
	{
		var token = "123";
		var requestsPerTimespan = new RequestsPerTimespan(2, 1000);
		var resource = Guid.NewGuid();
		var statisticsRepository = new AccessStatisticsStaticRepository();
		var accessService = new AccessService(resource, new IRule[] { requestsPerTimespan }, statisticsRepository);

		Assert.That(accessService.HasAccess(token), Is.True);
		Thread.Sleep(400);
		Assert.That(accessService.HasAccess(token), Is.True);
		Thread.Sleep(400);
		Assert.That(accessService.HasAccess(token), Is.False);
		Thread.Sleep(400);
		Assert.That(accessService.HasAccess(token), Is.True);
	}

	[Test]
	public void LocationBasedRule_ShouldFailWhenViolated()
	{
		var token = "123US";
		var locationBasedRule = new LocationBasedRule(2, 1000, 5000);
		var resource = Guid.NewGuid();
		var statisticsRepository = new AccessStatisticsStaticRepository();
		var accessService = new AccessService(resource, new IRule[] { locationBasedRule }, statisticsRepository);

		Assert.That(accessService.HasAccess(token), Is.True);
		Thread.Sleep(400);
		Assert.That(accessService.HasAccess(token), Is.True);
		Thread.Sleep(400);
		Assert.That(accessService.HasAccess(token), Is.False);
		Thread.Sleep(400);
		Assert.That(accessService.HasAccess(token), Is.True);
	}

	[Test]
	public void TotalCountRule_ShouldFailWhenViolated()
	{
		var token = "123";
		var totalCountRule = new TotalCount(2);
		var resource = Guid.NewGuid();
		var statisticsRepository = new AccessStatisticsStaticRepository();
		var accessService = new AccessService(resource, new IRule[] { totalCountRule }, statisticsRepository);

		Assert.That(accessService.HasAccess(token), Is.True);
		Assert.That(accessService.HasAccess(token), Is.True);
		Assert.That(accessService.HasAccess(token), Is.False);
	}
	
	[Test]
	public void RulesCombination_ShouldFailIfOneFail()
	{
		var token = "123";
		var resource = Guid.NewGuid();
		var totalCountRule = new TotalCount(2);
		var timePassedRule = new TimePassed(1000);
		var statisticsRepository = new AccessStatisticsStaticRepository();
		var accessService = new AccessService(resource, new IRule[] { totalCountRule, timePassedRule }, statisticsRepository);

		Assert.That(accessService.HasAccess(token), Is.True);
		Thread.Sleep(500);
		Assert.That(accessService.HasAccess(token), Is.False);
		Thread.Sleep(500);
		Assert.That(accessService.HasAccess(token), Is.True);
		Thread.Sleep(500);
		Assert.That(accessService.HasAccess(token), Is.False);
		Thread.Sleep(500);
		Assert.That(accessService.HasAccess(token), Is.False);
	}

	[Test]
	public void DifferentResources_ShouldNotInterfere()
	{
		var token = "123";
		var resourceA = Guid.NewGuid();
		var resourceB = Guid.NewGuid();
		var totalCountRule = new TotalCount(2);
		var timePassedRule = new TimePassed(1000);
		var statisticsRepository = new AccessStatisticsStaticRepository();
		var accessServiceA = new AccessService(resourceA, new IRule[] { totalCountRule }, statisticsRepository);
		var accessServiceB = new AccessService(resourceB, new IRule[] { timePassedRule }, statisticsRepository);

		Assert.That(accessServiceB.HasAccess(token), Is.True);
		Assert.That(accessServiceA.HasAccess(token), Is.True);
		Thread.Sleep(500);
		Assert.That(accessServiceB.HasAccess(token), Is.False);
		Assert.That(accessServiceA.HasAccess(token), Is.True);
		Thread.Sleep(500);
		Assert.That(accessServiceB.HasAccess(token), Is.True);
		Assert.That(accessServiceA.HasAccess(token), Is.False);
	}
}