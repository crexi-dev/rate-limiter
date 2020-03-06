using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Common
{
	public abstract class Rule
	{
		public abstract bool IsValidRequest();
	}
}
