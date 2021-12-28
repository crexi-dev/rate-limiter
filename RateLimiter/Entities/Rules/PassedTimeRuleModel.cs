using RateLimiter.Models;
using System;

namespace RateLimiter.Entities.Rules
{
    public class PassedTimeRuleModel: RuleModel
    {
        public LimitPeriodModel PassedTime { get; set; }

        public PassedTimeRuleModel()
        {
            PassedTime = new LimitPeriodModel();
        }

        public override bool CheckRule(ClientModel client)
        {
            if (client.LastCallDate.HasValue)
            {
                var blockEndDate = client.LastCallDate;
                if (PassedTime.DaysLimit.HasValue)
                {
                    blockEndDate = blockEndDate.Value.AddDays(PassedTime.DaysLimit.Value);
                }

                if (PassedTime.HoursLimit.HasValue)
                {
                    blockEndDate = blockEndDate.Value.AddHours(PassedTime.HoursLimit.Value);
                }

                if (PassedTime.MinutesLimit.HasValue)
                {
                    blockEndDate = blockEndDate.Value.AddMinutes(PassedTime.MinutesLimit.Value);
                }

                return DateTime.Now > blockEndDate;
            }

            return true;
        }
    }
}
