using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiterApi
{
	public interface IFilterRequest
	{
		public bool IsValidRequest(IUser client);
	}
}
