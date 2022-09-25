using System;

namespace RateLimiter.Models
{
    public class RequestsPerTimeSpanCount
	{
		public DateTimeOffset StartTime { get; set; }
		public int Count { get; set; }
	}
}
