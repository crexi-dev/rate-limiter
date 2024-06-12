using System;
using System.Linq;

namespace RateLimiter
{
	[TimeSpanPassedSinceLastCallRule("potatoes are limited in time", 5)]
	[XRequestsPerTimeSpanRule("potatoes are limited in quantity and in time", 4, 1)]
	public class Potato : RuledResource
	{
		private string _name;

		public Potato(string name)
		{
			_name = name;
		}

		public string Download(IRequest request)
		{
			var attributes = Attribute.GetCustomAttributes(typeof(Potato), typeof(ResourceRuleAttribute))
				.Select(i => i as ResourceRuleAttribute);

			if (AreAllRulesValid(attributes, request) == false)
				return null;

			return _name;
		}
	}
}
