using System;

namespace RateLimiter.Rules
{
    public static class RulesHelper
    {
        public static bool HasElapsedTime(DateTime prevReqDt, TimeSpan elapsedConfig)
        {
            var currTime = DateTime.UtcNow;

            var elapsedTime = currTime.Subtract(prevReqDt);

            var diffElapsed = elapsedConfig.Subtract(elapsedTime);

            return (diffElapsed.TotalMilliseconds < 0);
        }
    }
}