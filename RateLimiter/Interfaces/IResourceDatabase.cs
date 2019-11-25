using System;
using System.Collections.Generic;

namespace RateLimiter.Interfaces
{
	public interface IResourceDatabase
	{
		void Add(long resourceId, long userId, DateTime date);

		IEnumerable<ResourceAccess> GetAccesses(long resourceId, long userId);
	}
}
