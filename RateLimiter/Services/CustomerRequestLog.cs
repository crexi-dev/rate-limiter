using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
	public class CustomerRequestLog
	{
		public int Id { get; set; }
		public int CustomerId { get; set; }
		public DateTime RequestTime { get; set; }
		public string Request { get; set; }
	}
}
