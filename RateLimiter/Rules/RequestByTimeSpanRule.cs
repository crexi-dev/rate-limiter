using System;

namespace RateLimiter.Rules
{
    public class RequestByTimeSpanRule
    {
        private int _requestLimit;

        public RequestByTimeSpanRule(int requestLimit)
        {
            _requestLimit = requestLimit;
        }

        public bool Validate(int requestNumber)
        {
            return _requestLimit > requestNumber;
        }
    }
}
