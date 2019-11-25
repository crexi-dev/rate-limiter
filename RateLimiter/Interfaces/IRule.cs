using System;
using System.Collections.Generic;

namespace RateLimiter.Interfaces
{
	public interface IRule
	{
		bool CheckAccess(IEnumerable<ResourceAccess> data, DateTime date);
	}
}
