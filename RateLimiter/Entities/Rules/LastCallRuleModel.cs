using RateLimiter.Models;
using System;

namespace RateLimiter.Entities.Rules
{
    public class LastCallRuleModel: RuleModel
    {
        public LimitPeriodModel Limits { get; set; }

        public LastCallRuleModel()
        {
            Limits = new LimitPeriodModel();
        }

        public override bool CheckRule(ClientModel client)
        {
            var daysCheck = (Limits.DaysLimit.HasValue && client.LastCallDate.HasValue) ? client.LastCallDate.Value.AddDays(Limits.DaysLimit.Value) <= DateTime.Now : true;
            var hoursCheck = (Limits.HoursLimit.HasValue && client.LastCallDate.HasValue) ? client.LastCallDate.Value.AddHours(Limits.HoursLimit.Value) <= DateTime.Now : true;
            return daysCheck && hoursCheck;
        }
    }
}
