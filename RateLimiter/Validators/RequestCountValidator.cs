using RateLimiter.DataModel;
using RateLimiter.Extensions;

namespace RateLimiter.Validators
{
    public class RequestCountValidator : IRuleValidator
    {
        public RequestCountValidator()
        {

        }

        public int RuleId => 1;

        public bool Validate(RequestData requestData)
        {
            var historyData = requestData.GetRelevantLatestHistoryData();

            if (historyData != null)
            {
                if (requestData.RequestedTime.Date == historyData.LastRequested.Date && requestData.RequestedTime.Hour == historyData.LastRequested.Hour && requestData.RequestedTime.Minute == historyData.LastRequested.Minute)
                {
                    if (historyData.Count >= requestData.GetRuleValue(RuleId))
                        return false;
                }
            }
            return true;
        }
    }
}
