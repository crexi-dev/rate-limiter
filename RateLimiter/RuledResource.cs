using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
	public class RuledResource
	{
		protected bool AreAllRulesValid(IEnumerable<ResourceRuleAttribute> rules, IRequest request)
		{
			if (rules == null || rules.Any() == false)
			{
				return true;
			}

			foreach (ResourceRuleAttribute rule in rules)
			{
				if (rule.IsValid(request) == false)
				{
					return false;
				}
			}

			return true;
		}
	}
}
