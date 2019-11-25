using System;
using System.Collections.Generic;

namespace RateLimiter.Interfaces
{
	public interface IRulesManager
	{
		bool CheckAccess(IEnumerable<ResourceAccess> data, DateTime date);
	}
}
