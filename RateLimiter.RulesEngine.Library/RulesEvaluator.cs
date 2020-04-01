using RateLimiter.Library;

namespace RateLimiter.RulesEngine.Library
{
    public class RulesEvaluator : IRulesEvaluator
    {
        public RateLimitSettingsConfig Evaluate(string resourceName, string IPAddress)
        {
            // evaluate rules in order of specificity:

            // 1. lookup resource/region rules
            // return if found

            // 2. lookup resource rules
            // return if found

            // 3. lookup region rules

            throw new System.NotImplementedException();
        }
    }
}
