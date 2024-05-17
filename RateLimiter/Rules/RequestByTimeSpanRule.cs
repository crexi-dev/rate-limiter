using RateLimiter.Guards;
using RateLimiter.Models;
using RateLimiter.Rules.Info;
using System;

namespace RateLimiter.Rules
{
    public class RequestByTimeSpanRule : IRule 
    {
        private int _requestLimit;

        public RequestByTimeSpanRule(int requestLimit)
        {
            _requestLimit = requestLimit;
        }

        public bool Validate(RuleRequestInfo? requestInfo)
        {
            Guard.RequestInfoType<RequestByTimeSpanRuleInfo>(requestInfo);
            var info = (RequestByTimeSpanRuleInfo)requestInfo;
            return _requestLimit < info.Requests;
            
        }
    }
}
