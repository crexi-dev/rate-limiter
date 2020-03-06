using RateLimiter.Common;
using RateLimiter.Services;
using System;
using System.Linq;

namespace RateLimiter.Rules
{
    public class MaximumReqInPeriodRule : Rule
    {
        //maximum request allowed in a period
        public int MaxRequestLimit { get; } = 3;

        // period in seconds
        public int Period { get; } = 30;

        private readonly int _customerId;
        private readonly ApiDataService _service;

        public MaximumReqInPeriodRule(int customerId, ApiDataService service)
        {
            _customerId = customerId;
            _service = service;
        }

        public override bool IsValidRequest()
        {
            var logs = _service.GetRequestLogs(_customerId);
            if (logs != null)
            {
                var count = logs.Where(l => l.CustomerId == _customerId && l.RequestTime > DateTime.Now.AddSeconds(-1 * Period)).Count();
                if (count >= MaxRequestLimit)
                {
                    Console.WriteLine("MinimumIntervalRule Failed");
                    return false;
                }
            }
            return true;
        }
    }

}
