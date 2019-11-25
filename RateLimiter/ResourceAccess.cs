using System;

namespace RateLimiter
{
	public class ResourceAccess
	{
		public long ResourceId { get; set; }

		public long UserId { get; set; }

		public DateTime AccessOn { get; set; }
	}
}
