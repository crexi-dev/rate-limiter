using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Rules
{
    public class RequestSinceLastCallRule : IRule
    {
        private readonly TimeSpan _requestSinceLastTimeSpan;

        public RequestSinceLastCallRule(TimeSpan requestSinceLast)
        {
            _requestSinceLastTimeSpan = requestSinceLast;
        }

        public bool Verify(Stack<DateTimeOffset> requestTimes, DateTimeOffset current)
        {
            var floor = current.ToUniversalTime() - _requestSinceLastTimeSpan;

            if (requestTimes.Any())
            {
                return floor > requestTimes.Pop().ToUniversalTime();
            }

            return true;
        }
    }
}
