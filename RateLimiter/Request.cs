using System;

namespace RateLimiter
{
	public class Request : IRequest
	{
		public string IpAddress { get; set; }

		public DateTime TimeStamp { get; set; }
	}
}
