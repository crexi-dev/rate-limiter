using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
	public class IntervalRule : IRule
	{
		private readonly TimeSpan Limit = new TimeSpan(0,0,2);

		public bool IsMatch(IList<DateTime> times)
		{
			var currentTime = times.Last();
			if(times.Count <= 1) return true;

			var prevTime = times[times.Count - 2];

			return currentTime.Subtract(prevTime) >= Limit;
		}
	}
}
