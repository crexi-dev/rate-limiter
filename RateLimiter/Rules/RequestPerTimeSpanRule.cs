using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Rules
{
    public class RequestPerTimeSpanRule : IRule
    {
        private readonly int _requestsLimit;
        private readonly TimeSpan _requestTimeSpanLimit;

        public RequestPerTimeSpanRule(int requestsLimit, TimeSpan requestTimeSpanLimit)
        {
            _requestsLimit = requestsLimit;
            _requestTimeSpanLimit = requestTimeSpanLimit;
        }

        public bool Verify(Stack<DateTimeOffset> requestTimes, DateTimeOffset current)
        {
            var floor = current.ToUniversalTime() - _requestTimeSpanLimit;
            var counter = 0;


            while (requestTimes.Any() && requestTimes.Pop().ToUniversalTime() > floor)
            {
                counter++;
            }

            return _requestsLimit > counter;
        }
    }
}
