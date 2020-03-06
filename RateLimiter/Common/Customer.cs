using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Common
{
	public class Customer
	{
		public int Id { get; set; }
		public string AccessToken { get; set; }
		public string Region { get; set; }

		public string[] Rules { get; set; }
	}
}
