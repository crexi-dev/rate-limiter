using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class IntervalRateLimiter : IRateLimiterService
    {
        public override bool ShouldDenyExecution(Guid accessToken)
        {
            //Get time of initial request
            var timeOfRequest = DateTime.Now;
            var shouldDeny = false;

            var lastRequestByAccessToken = Requests
                .Where(r => r.Key == accessToken)
                .OrderByDescending(r => r.Value)
                .FirstOrDefault();
            if (lastRequestByAccessToken.Value == new DateTime() || (timeOfRequest - lastRequestByAccessToken.Value).TotalMilliseconds > TIME_INTERVAL)
            {
                //First request needs to be added
                Requests.Add(new KeyValuePair<Guid, DateTime>(accessToken, DateTime.Now));
            }
            else
            {
                //Too soon, deny request
                shouldDeny = true;
            }
            LogResultToConsole(accessToken, timeOfRequest, shouldDeny);
            return shouldDeny;
        }
    }
}
