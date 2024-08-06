using RateLimiter.Configs;
using System;

namespace RateLimiter.Rules
{
    public class RulesEvaluator : IRulesEvaluator
    {
        IRateLimiterConfigs config;
        public RulesEvaluator(IRateLimiterConfigs _config)
        {
            config = _config;
        }

        /// <summary>
        /// if the user send more calls than max allowed PER timeframe window 
        /// ex: 11 calls per 3 seconds,where the rules are 10 max calls per 5 seconds
        /// CanAccess should return false
        /// </summary>
        /// <param name="start">first calls in a timeframe</param>
        /// <param name="last">last call recorded in a timeframe</param>
        /// <param name="totalCalls">number of calls per timeframe</param>
        /// <returns></returns>
        public bool CanAccess(DateTime start, DateTime last, int totalCalls)
        {
            var configs = config.BindConfig();

            /// isEnabled
            if (configs?.Enabled == true)
            {
                if (configs.MaxAllowed.HasValue && configs.TimeFrame.HasValue)
                {
                     int MaxAllowedCalls = configs.MaxAllowed.Value;
                     int PerTimeFrame = configs.TimeFrame.Value;
                
                     // evaluate
                     var difference = (last - start).TotalSeconds;
                
                     /// the user timespan was within limit to evaluate
                     if (difference <= PerTimeFrame && totalCalls > MaxAllowedCalls)
                     {
                         // user called the APIs > listed in config 
                         return false;
                     }
                     return true;
                 }
            }

            return true;
        }
    }

}
