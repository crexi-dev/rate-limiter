using RateLimiter.Common;
using RateLimiter.Services;
using System;
using System.Linq;

namespace RateLimiter.Rules
{
	public class MinimumIntervalRule : Rule
	{
		//minimum time interval required between each request
		// interval in milli seconds
		private readonly int _minInterval;
		private readonly int _customerId;
		private readonly ApiDataService _service;

		public MinimumIntervalRule(int customerId, ApiDataService service, int minInterval = 2000)
		{
			_minInterval = minInterval;
			_customerId = customerId;
			_service = service;
		}

		public override bool IsValidRequest()
		{
			var logs = _service.GetRequestLogs(_customerId);
			if (logs != null)
			{
				var log = logs.Where(l => l.CustomerId == _customerId).OrderByDescending(l => l.Id).Take(1).SingleOrDefault();

				if (log != null)
				{
					if(!(log.RequestTime < DateTime.Now.AddMilliseconds(-1 * _minInterval)))
					{
						Console.WriteLine("MinimumIntervalRule Failed");
						return false;
					}
				}
			}
			return true;
		}
	}

}
