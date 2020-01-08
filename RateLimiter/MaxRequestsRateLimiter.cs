using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class MaxRequestsRateLimiter : IntervalRateLimiter
    {
        public override bool ShouldDenyExecution(Guid accessToken)
        {
            //Get time of initial request
            var timeOfRequest = DateTime.Now;
            var shouldDeny = false;

            //Take up to the Max requests alloted
            var requestsFromAccessToken = Requests
                    .Where(r => r.Key == accessToken)
                    .OrderByDescending(r => r.Value)
                    .Take(MAX_REQUESTS);
            if (requestsFromAccessToken.Count() == MAX_REQUESTS
                && (timeOfRequest - requestsFromAccessToken.Last().Value).TotalMilliseconds < TIME_INTERVAL)
            {
                //Earliest of Max requests is too close to most recent.  Cannot process
                shouldDeny = true;
            }
            else
            {
                //Add last request to dictionary
                Requests.Add(new KeyValuePair<Guid, DateTime>(accessToken, DateTime.Now));
            }

            LogResultToConsole(accessToken, timeOfRequest, shouldDeny);
            return shouldDeny;
        }
    }
}
