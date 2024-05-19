using GuardNet;
using RateLimiter.CustomGuards;
using RateLimiter.Rules.Info;

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
            Guard.NotNull(requestInfo, nameof(requestInfo));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            CustomGuard.IsValidRuleRequestInfoType<RequestByTimeSpanRuleInfo>(requestInfo.GetType());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            
            var info = (RequestByTimeSpanRuleInfo)requestInfo;
            return _requestLimit < info.Requests;
            
        }
    }
}
