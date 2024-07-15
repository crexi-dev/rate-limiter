using System;
using System.Collections.Generic;

namespace RateLimiter.Extensions
{
    public static class ParametersExtensions
    {
        public static uint ToUInt(this IDictionary<string, object> parameters, string key)
        {
            ValidateParameter(parameters, key);

            return Convert.ToUInt32(parameters[key]);
        }

        public static DateTime ToDate(this IDictionary<string, object> parameters, string key)
        {
            ValidateParameter(parameters, key);

            return DateTime.Parse((string)parameters[key], null, System.Globalization.DateTimeStyles.RoundtripKind);
        }

        private static void ValidateParameter(IDictionary<string, object> parameters, string key)
        {
            if (!parameters?.ContainsKey(key) ?? false)
            {
                throw new ArgumentException($"Missing parameter {key}");
            }
        }
    }
}
