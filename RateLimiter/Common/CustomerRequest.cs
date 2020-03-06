using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Common
{
	public class CustomerRequest
	{
		public int CustomerId { get; set; }
		public string Request { get; set; }
	}
}
