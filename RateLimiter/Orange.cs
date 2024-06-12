using System;
using System.Linq;

namespace RateLimiter
{
	[XRequestsPerTimeSpanRule("oranges are limited in quantity and time", 4, 2)]
	public class Orange : RuledResource
	{
		private string _name;

		public Orange(string name)
		{
			_name = name;
		}

		public string Download(IRequest request)
		{
			var attributes = Attribute.GetCustomAttributes(typeof(Orange), typeof(ResourceRuleAttribute))
				.Select(i => i as ResourceRuleAttribute);

			if (AreAllRulesValid(attributes, request) == false)
				return null;

			return _name;
		}
	}
}
