using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiterApi
{
	public static class RuleConfig
	{
		public static readonly Dictionary<Rules, int> AttemptsByRules = new Dictionary<Rules, int>
		{
			{ Rules.RuleA, 2 },
			{ Rules.RuleB, 3 },
			{ Rules.RuleC, 4 },
		};
	}
}
