using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Interfaces;
using RateLimiter.Rules;

namespace RateLimiter
{
	public class RulesManager : IRulesManager
	{
		private readonly List<IRule> rules = new List<IRule>();

		public RulesManager()
		{
			rules.Add(new RequestsPerTimespanRule());
		}

		public bool CheckAccess(IEnumerable<ResourceAccess> data, DateTime date)
		{
			return rules.All(rule => rule.CheckAccess(data, date));
		}
	}
}
