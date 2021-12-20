using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiterApi
{
	public class Client : IUser
	{
		public Rules Rule { get; set; }
		public int Id { get; set; }
	}
}
