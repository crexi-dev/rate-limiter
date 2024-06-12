using RateLimiter.RateLimiters.Rules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.RateLimiters;

public sealed class InMemoryRulesStorage : IRulesStorage
{
	private readonly IDictionary<string, IRule> _rules = new Dictionary<string, IRule>
	{
		{"image1.png", new RequestPerTimespanRule(TimeSpan.FromSeconds(1), 10)},
		{"image2.png", new RegionBaseRule(new RequestPerTimespanRule(TimeSpan.FromSeconds(2),5), new TimespanBetweenCallsRule(TimeSpan.FromSeconds(1))) },
		{"api/controller", new OrRule(new RequestPerTimespanRule(TimeSpan.FromSeconds(1),10), new TimespanBetweenCallsRule(TimeSpan.FromSeconds(5))) }
	};

	public Task<IRule> GetRule(string resource)
	{
		if (_rules.ContainsKey(resource))
		{
			return Task.FromResult(_rules[resource]);
		}

		return Task.FromResult(NullRule.Null);
	}
}
