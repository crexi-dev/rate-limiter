using System;

namespace RateLimiter
{
	public interface IRequest
	{
		string IpAddress { get; set; }

		DateTime TimeStamp { get; set; }
	}
}
