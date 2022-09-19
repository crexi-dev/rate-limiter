using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Rules
{
    public class RequestTotalSizePeriodLimiteRule : ILimiteRule
    {
        private object syncObject = new object();

        private List<(DateTime, long)> log;
        private long periodMs;
        private long sizeLimit;

        public RequestTotalSizePeriodLimiteRule(long _periodMs, long _sizeLimit)
        {
            log = new List<(DateTime, long)>();

            periodMs = _periodMs;
            sizeLimit = _sizeLimit;
        }

        public bool CanPassNow(Request request)
        {
            if (request.Size > sizeLimit)
            {
                return false;
            }

            lock (syncObject)
            {
                var currentPassedSize = log.Where(x => x.Item1 > DateTime.Now.AddMilliseconds(-periodMs)).Sum(x => x.Item2);
                if (currentPassedSize + request.Size <= sizeLimit)
                {
                    log.Add((DateTime.Now, request.Size));
                    return true;
                }
                else
                {
                    //TODO Remove old values from log
                    return false;
                }
            }
        }
    }
}
