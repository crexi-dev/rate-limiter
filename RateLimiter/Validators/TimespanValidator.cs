using System;
using RateLimiter.DataModel;
using RateLimiter.Extensions;

namespace RateLimiter.Validators
{
    public class TimespanValidator : IRuleValidator
    {
        public TimespanValidator()
        {

        }

        public int RuleId => 2;

        public bool Validate(RequestData requestData)
        {
            var historyData = requestData.GetRelevantLatestHistoryData();

            if (historyData != null)
            {
                TimeSpan ts = requestData.RequestedTime - historyData.LastRequested;
                if (ts.TotalSeconds < requestData.GetRuleValue(RuleId))
                    return false;
            }

            return true;
        }
    }
}
