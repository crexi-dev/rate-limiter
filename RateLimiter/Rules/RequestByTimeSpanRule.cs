using RateLimiter.Models;
using System;

namespace RateLimiter.Rules
{
    public class RequestByTimeSpanRule : IRule 
    {
        private int _requestLimit;

        public RequestByTimeSpanRule(int requestLimit, int timeSpan)
        {
            _requestLimit = requestLimit;
        }

        public bool Validate(Request request)
        {
            return true;
        }
    }
}
