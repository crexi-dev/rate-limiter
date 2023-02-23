using System;
using System.Collections.Generic;

namespace RateLimiter.Models
{
	public class RequestsData
	{
		public List<DateTime> Requests { get; set; } = new ();
	}
}