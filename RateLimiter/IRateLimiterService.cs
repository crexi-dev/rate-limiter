using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public abstract class IRateLimiterService
    {
        internal static List<KeyValuePair<Guid, DateTime>> Requests = new List<KeyValuePair<Guid, DateTime>>();

        //This would best be moved into a config file, or maybe even a lookup table where clients can each have their own minimum time interval
        internal const int TIME_INTERVAL = 5000;
        internal const int MAX_REQUESTS = 5;

        public abstract bool ShouldDenyExecution(Guid accessToken);

        internal static void LogResultToConsole(Guid accessToken, DateTime timeOfRequest, bool shouldDeny)
        {
            var stringResult = shouldDeny ? "denied" : "accepted";
            Console.WriteLine("Request " + stringResult
                + " for Access Token " + accessToken
                + " at " + timeOfRequest.ToLongTimeString());
        }
    }
}
