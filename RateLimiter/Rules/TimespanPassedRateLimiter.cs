using RateLimiter.Enums;
using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;

namespace RateLimiter.Rules
{
    /// <summary>
    /// A certain timespan has passed since the last call
    /// </summary>
    public class TimespanPassedRateLimiter : IRateLimiter
    {
        // NOTE
        // This value must come from a config file, app settings or parameter store in the cloud
        // For this assignment I am using constants
        // I am also assuming that this time span is in minutes
        private double TimespanPassed = 1;

        private bool result = false;

        public bool Acquire(ServiceType serviceType, string userToken, DateTime requestedDate)
        {
            lock (InMemoryStorage.requestLogByService)
            {
                // If a service hasnt been accessed yet, initiaize it in the dictionary
                if (!InMemoryStorage.requestLogByService.ContainsKey(serviceType))
                    InMemoryStorage.requestLogByService[serviceType] = new Dictionary<string, RequestWindow>();

                // If this user is requesting this service for the first time or a new fixed window needs to be initialized
                RequestWindow window;
                InMemoryStorage.requestLogByService[serviceType].TryGetValue(userToken, out window);

                if (window == null)
                {
                    InMemoryStorage.requestLogByService[serviceType][userToken] = new RequestWindow(requestedDate, 1);
                    result = true;
                }
                else
                {
                    DateTime minimumRequiredTimeSinceLastRequest = window.timestamp.AddMinutes(TimespanPassed);

                    if (requestedDate >= minimumRequiredTimeSinceLastRequest) 
                    {
                        InMemoryStorage.requestLogByService[serviceType][userToken] = new RequestWindow(requestedDate, 1);
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }

            return result;
        }
    }
}
