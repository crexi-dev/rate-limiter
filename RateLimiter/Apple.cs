using System;
using System.Linq;

namespace RateLimiter
{
	[TimeSpanPassedSinceLastCallRule("apple are limited in time", 2)]
	public class Apple : RuledResource
	{
		private string _name;

		public Apple(string name)
		{
			_name = name;
		}

		public string Download(IRequest request)
		{
			var attributes = Attribute.GetCustomAttributes(typeof(Apple), typeof(ResourceRuleAttribute))
				.Select(i => i as ResourceRuleAttribute);

			if (AreAllRulesValid(attributes, request) == false)
				return null;

			return _name;
		}
	}

}
