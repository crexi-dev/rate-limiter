using System;

namespace RateLimiter
{
	public class ResourceRuleAttribute : Attribute
	{
		public virtual bool IsValid(IRequest payload) { return true; }
	}
}
