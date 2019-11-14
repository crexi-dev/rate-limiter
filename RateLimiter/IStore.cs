using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
	public interface IStore
	{
		void Save(string token);
		IList<DateTime> Get(string token);
	}
}
