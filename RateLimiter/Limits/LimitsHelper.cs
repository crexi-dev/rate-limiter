using System.Collections.Generic;

namespace RateLimiter.Limits
{
    public static class LimitsHelper
    {
        public static bool CanInvoke(IEnumerable<ILimitByClient> limits, string clientId)
        {
            var overallResult = true;

            foreach (var limit in limits)
            {
                var result = limit.CanInvoke(clientId);

                if (!result)
                {
                    overallResult = false;
                }
            }

            return overallResult;
        }
    }
}
