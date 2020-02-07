using RateLimiter.Api.Attributes;
using RateLimiter.Api.Controllers;
using RateLimiter.Enums;
using RateLimiter.Utilities;
using System;
using System.Diagnostics;
using System.Reflection;

namespace RateLimiter.Api.Validators
{
    public class RequestValidator
    {
        public static bool ValidateRequest()
        {
            var attribute = GetAttribute(typeof(ValuesController));

            if (attribute == null)
            {
                return true;
            }

            if (!attribute.ApplyQuantityRules && !attribute.ApplyTimeRules)
            {
                return true;
            }
            else if (attribute.ApplyQuantityRules && attribute.ApplyTimeRules)
            {
                try
                {
                    return RuleValidator.ValidateByType(RuleTypes.FrequencyTimeSpanLimiting);
                }
                catch (Exception)
                {
                    Debug.WriteLine($"Validation failed for the rule {RuleTypes.FrequencyLimiting}");

                    return false;
                }
            }
            else if (attribute.ApplyQuantityRules)
            {
                try
                {
                    return RuleValidator.ValidateByType(RuleTypes.FrequencyLimiting);
                }
                catch (Exception)
                {
                    Debug.WriteLine($"Validation failed for the rule {RuleTypes.FrequencyLimiting}");

                    return false;
                }
            }
            else
            {
                try
                {
                    return RuleValidator.ValidateByType(RuleTypes.TimeSpanLimiting);
                }
                catch (Exception)
                {
                    Debug.WriteLine($"Validation failed for the rule {RuleTypes.TimeSpanLimiting}");

                    return false;
                }
            }
        }

        private static ValidateRateLimitAttribute GetAttribute(MemberInfo memberInfo)
        {
            // Get instance of the attribute.
            return (ValidateRateLimitAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(ValidateRateLimitAttribute));
        }
    }
}
