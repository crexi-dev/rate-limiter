using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Interfaces;

namespace RateLimiter
{
	public class ResourceDatabase : IResourceDatabase
	{
		private readonly List<ResourceAccess> data = new List<ResourceAccess>();
		
		public void Add(long resourceId, long userId, DateTime date)
		{
			data.Add(new ResourceAccess
			{
				ResourceId = resourceId,
				UserId = userId,
				AccessOn = date
			});
		}

		public IEnumerable<ResourceAccess> GetAccesses(long resourceId, long userId)
		{
			return data.Where(item => item.ResourceId == resourceId && item.UserId == userId);
		}
	}
}
