using Moq;
using NUnit.Framework;
using RateLimiter.Rules;
using System.Collections.Generic;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
	private readonly List<Mock<IRateLimitRule>> _rateLimitRuleMocks = new List<Mock<IRateLimitRule>>
	{
		new(),
		new()
	};

    [Test]
    public void IsRequestAllowed_NoRulesForResource_ReturnsTrue()
    {
        // arrange
        string resource = "resource";
        string token = "token";

        var rateLimiter = new RateLimiter(new Dictionary<string, ICollection<IRateLimitRule>>
        {
        });

        // act
        // assert
        Assert.True(rateLimiter.IsRequestAllowed(token, resource));
    }

    [Test]
    public void IsRequestAllowed_AllRulesPass_ReturnsTrue()
    {
        // arrange
        string resource = "resource";
        string token = "token";
        var key = $"{resource}:{token}";

        _rateLimitRuleMocks[0].Setup(x => x.IsRequestAllowed(resource, token)).Returns(true);
        _rateLimitRuleMocks[1].Setup(x => x.IsRequestAllowed(resource, token)).Returns(true);

        var rateLimiter = new RateLimiter(new Dictionary<string, ICollection<IRateLimitRule>>
        {
            {
                resource,
                new List<IRateLimitRule> { _rateLimitRuleMocks[0].Object, _rateLimitRuleMocks[1].Object }
            }
        });

        // act
        // assert
        Assert.True(rateLimiter.IsRequestAllowed(token, resource));
    }

    [Test]
	public void IsRequestAllowed_OneRuleFails_ReturnsTrue()
	{
		// arrange
		string resource = "resource";
		string token = "token";
        var key = $"{resource}:{token}";

        _rateLimitRuleMocks[0].Setup(x => x.IsRequestAllowed(resource, token)).Returns(true);
        _rateLimitRuleMocks[1].Setup(x => x.IsRequestAllowed(resource, token)).Returns(false);

        // act
        var rateLimiter = new RateLimiter(new Dictionary<string, ICollection<IRateLimitRule>>
		{
			{
				resource,
				new List<IRateLimitRule> { _rateLimitRuleMocks[0].Object, _rateLimitRuleMocks[1].Object }
            }
        });

        // act
        // assert
        Assert.False(rateLimiter.IsRequestAllowed(resource, token));
	}

    [TestCase(false)]
    [TestCase(true)]
    public void AddRule_AddRuleForResource_NewRuleExecuted(bool isRequestAllowed)
    {
        // arrange
        string resource = "resource";
        string token = "token";

        var rateLimiter = new RateLimiter(new Dictionary<string, ICollection<IRateLimitRule>>
        {
        });

        _rateLimitRuleMocks[0].Setup(x => x.IsRequestAllowed(resource, token)).Returns(isRequestAllowed);

        // act
        rateLimiter.AddRule(resource, _rateLimitRuleMocks[0].Object);

        // assert
        Assert.AreEqual(rateLimiter.IsRequestAllowed(resource, token), isRequestAllowed);
    }

    [Test]
    public void AddRule_AddRuleForResourceWithExistingRules_NewRuleExecuted()
    {
        // arrange
        string resource = "resource";
        string token = "token";

        _rateLimitRuleMocks[0].Setup(x => x.IsRequestAllowed(resource, token)).Returns(true);
        _rateLimitRuleMocks[1].Setup(x => x.IsRequestAllowed(resource, token)).Returns(false);

        var rateLimiter = new RateLimiter(new Dictionary<string, ICollection<IRateLimitRule>>
        {
            { resource, new List<IRateLimitRule> { _rateLimitRuleMocks[0].Object } }
        });

        // act
        rateLimiter.AddRule(resource, _rateLimitRuleMocks[1].Object);

        // assert
        Assert.AreEqual(rateLimiter.IsRequestAllowed(resource, token), false);
    }
}