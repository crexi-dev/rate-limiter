using RateLimiter.Data;
using RateLimiter.Enums;
using System;

namespace RateLimiter.Utilities
{
    public static class RuleValidator
    {
        public static bool ValidateByType(RuleTypes ruleType)
        {
            switch (ruleType)
            {
                case RuleTypes.FrequencyLimiting:
                    {
                        return HistoryHelper.ValidateFrequency(SeedData.Token1, SeedData.FrequencyLimitRule.NumberOfRequests, SeedData.FrequencyLimitRule.TimeSpanLimit);
                    }
                case RuleTypes.TimeSpanLimiting:
                    {
                        return HistoryHelper.ValidateTimeSpan(SeedData.Token1, SeedData.TimeSpanLimitRule.TimeSpanLimit);

                    }
                case RuleTypes.FrequencyTimeSpanLimiting:
                    {
                        return HistoryHelper.ValidateFrequency(SeedData.Token1, SeedData.FrequencyLimitRule.NumberOfRequests, SeedData.FrequencyLimitRule.TimeSpanLimit)
                            && HistoryHelper.ValidateTimeSpan(SeedData.Token1, SeedData.TimeSpanLimitRule.TimeSpanLimit);
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(ruleType), ruleType,
                            "The specified value of RuleTypes does not exist.");
                    }
            }
        }
    }
}
