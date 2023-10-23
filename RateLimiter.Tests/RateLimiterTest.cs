using NUnit.Framework;
using RateLimiter.Contracts;
using RateLimiter.Models;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
    List<IClient> _clients = new List<IClient>()
    {   
        new Client(Guid.NewGuid(), "UsUser", 0),
        new Client(Guid.NewGuid(), "EuUser", 1),
        new Client(Guid.NewGuid(), "AsiaUser", 2),
        new Client(Guid.NewGuid(), "AustraliaUser2", 3)
    };

    IClient UsUser => _clients[0];
    IClient EuUser => _clients[1];
    IClient AustraliaUser2 => _clients[3];
    IClient AsiaUser => _clients[2];

    IRateLimiter _rateLimiter; 

    public RateLimiterTest()
    {
        var settings = new RateLimiterSettings()
        {
            Rules = new List<IRequestLimitRule>()
            {
                new RequestLimitRule
                {
                    RuleType = (byte)RequestLimitRuleType.RequestsPerTime,
                    CountLimit = 1,
                    Time = TimeSpan.FromSeconds(1),
                    RegionId = 0,
                    ResourceType =  (byte)ResourcesType.First
                },

                new RequestLimitRule
                {
                    RuleType = (byte)RequestLimitRuleType.TiemAfterLastCall,
                    Time = TimeSpan.FromSeconds(1),
                    RegionId = 1,
                    ResourceType = (byte)ResourcesType.First
                },

                new RequestLimitRule
                {
                    RuleType = (byte)RequestLimitRuleType.TiemAfterLastCall | (byte)RequestLimitRuleType.RequestsPerTime,
                    Time = TimeSpan.FromSeconds(1),
                    CountLimit = 1,
                    ResourceType = (byte)ResourcesType.Second | (byte)ResourcesType.Third
                }
            }
        };

        var factory = new DummyRateLimiterFacroty(settings, _clients);
        _rateLimiter = factory.GenerateRateLimitter();
    }

    [Test]
	public void Test0_OneResourceAccessAllowed()
	{
        var timeNow = (new DateTime(2000,1,1)).ToUniversalTime();
        var resource = ResourcesType.First;

        var a  = _rateLimiter.Validate(UsUser.Token, timeNow, resource);

		Assert.That(a, Is.True);

        var b = _rateLimiter.Validate(EuUser.Token, timeNow, resource);

        Assert.That(b, Is.True);
    }

    [Test]
    public void Test1_OneResourceAccesDenied_RequestsPerTime()
    {
        var timeNow = (new DateTime(2000, 2, 1)).ToUniversalTime();
        var resource = ResourcesType.First;

        var a = _rateLimiter.Validate(UsUser.Token, timeNow.Subtract(TimeSpan.FromMilliseconds(10)), resource);

        Assert.That(a, Is.True);

        a = _rateLimiter.Validate(UsUser.Token, timeNow, resource);

        Assert.That(a, Is.False);
    }

    [Test]
    public void Test2_OneResourceAccesDenied_TiemAfterLastCall()
    {
        var timeNow = (new DateTime(2000, 2, 1)).ToUniversalTime();
        var resource = ResourcesType.First;

        var b = _rateLimiter.Validate(EuUser.Token, timeNow, resource);

        Assert.That(b, Is.True);

        b = _rateLimiter.Validate(EuUser.Token, timeNow.Add(TimeSpan.FromMilliseconds(20)), resource);

        Assert.That(b, Is.False);
    }

    [Test]
    public void Test3_OneResourceAccessDenitedAnotherAccessAllowed_RequestsPerTime()
    {
        var timeNow = (new DateTime(2000, 3, 1)).ToUniversalTime();
        var resource = ResourcesType.First;
        var resource2 = ResourcesType.Second;

        var a = _rateLimiter.Validate(UsUser.Token, timeNow.Subtract(TimeSpan.FromMilliseconds(10)), resource);

        Assert.That(a, Is.True);

        a = _rateLimiter.Validate(UsUser.Token, timeNow, resource);

        Assert.That(a, Is.False);

        a = _rateLimiter.Validate(UsUser.Token, timeNow, resource2);

        Assert.That(a, Is.True);
    }

    [Test]
    public void Test4_TwoResourceForAllRegionMultyRuleAccessDenied_RequestsPerTime()
    {
        var timeNow = (new DateTime(2000, 4, 1)).ToUniversalTime();
        var resource2 = ResourcesType.Second;
        var resource3 = ResourcesType.Third;
        var resource1 = ResourcesType.First;

        var a = _rateLimiter.Validate(AsiaUser.Token, timeNow.Subtract(TimeSpan.FromMilliseconds(10)), resource3);
        Assert.That(a, Is.True);

        a = _rateLimiter.Validate(AsiaUser.Token, timeNow, resource3);
        Assert.That(a, Is.False);

        a = _rateLimiter.Validate(AsiaUser.Token, timeNow, resource2);
        Assert.That(a, Is.True); 

        a = _rateLimiter.Validate(AsiaUser.Token, timeNow.Add(TimeSpan.FromMilliseconds(10)), resource2);
        Assert.That(a, Is.False);

        _rateLimiter.Validate(AsiaUser.Token, timeNow, resource1);
        _rateLimiter.Validate(AsiaUser.Token, timeNow.Add(TimeSpan.FromMilliseconds(10)), resource1);
        _rateLimiter.Validate(AsiaUser.Token, timeNow.Add(TimeSpan.FromMilliseconds(20)), resource1);
        a = _rateLimiter.Validate(AsiaUser.Token, timeNow.Add(TimeSpan.FromMilliseconds(30)), resource1);
        Assert.That(a, Is.True);
    }

    [Test]
    public void Test5_TwoResourceForAllRegionMultyRuleAccessDenied_TiemAfterLastCall()
    {
        var timeNow = (new DateTime(2000, 4, 1)).ToUniversalTime();

        var resource2 = ResourcesType.Second;
        var resource3 = ResourcesType.Third;
        var resource1 = ResourcesType.First;

        var a = _rateLimiter.Validate(AustraliaUser2.Token, timeNow, resource3);
        Assert.That(a, Is.True);

        a = _rateLimiter.Validate(AustraliaUser2.Token, timeNow.Add(TimeSpan.FromMilliseconds(10)), resource3);
        Assert.That(a, Is.False);

        a = _rateLimiter.Validate(AustraliaUser2.Token, timeNow.Subtract(TimeSpan.FromMilliseconds(10)), resource2);
        Assert.That(a, Is.True);

        a = _rateLimiter.Validate(AustraliaUser2.Token, timeNow, resource2);
        Assert.That(a, Is.False);

        _rateLimiter.Validate(AustraliaUser2.Token, timeNow, resource1);
        _rateLimiter.Validate(AustraliaUser2.Token, timeNow.Add(TimeSpan.FromMilliseconds(10)), resource1);
        _rateLimiter.Validate(AustraliaUser2.Token, timeNow.Add(TimeSpan.FromMilliseconds(20)), resource1); 
        a = _rateLimiter.Validate(AustraliaUser2.Token, timeNow.Add(TimeSpan.FromMilliseconds(30)), resource1);
        Assert.That(a, Is.True);
    }

    [Test]
    public void Test7_UnknownUser()
    {
        var timeNow = (new DateTime(2000, 1, 1)).ToUniversalTime();
        var resource = ResourcesType.First;

        var ex = Assert.Throws<ArgumentException>(
            () => _rateLimiter.Validate(Guid.NewGuid(), timeNow, resource)
            );

        Assert.That(ex.Message, Is.EqualTo("Unknown client!"));
    }
}