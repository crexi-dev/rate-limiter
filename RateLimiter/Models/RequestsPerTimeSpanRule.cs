using System;

namespace RateLimiter.Models
{
    public class RequestsPerTimeSpanRule
	{
		public int AllowedNumberOfRequests { get; set; }
		public TimeSpan TimeSpan { get; set; }
	}
}
