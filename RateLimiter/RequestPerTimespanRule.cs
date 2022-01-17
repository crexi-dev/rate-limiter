using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public class RequestPerTimespanRule : IRule
    {
        private const int requestPerTimespan = 10;
        private TimeSpan timeSpan = new TimeSpan(1, 0, 0); 
        public bool CanAccess(string token, ClientLogsStorage storage)
        {
            int quantity = storage.LogQuantityByDateTime(token, DateTime.Now-timeSpan);
            return requestPerTimespan > quantity;
        }
    }
}
