using RateLimiter.Enums;
using RateLimiter.Interfaces;
using System.Collections.Generic;
using System;

namespace RateLimiter.Rules
{
    /// <summary>
    /// X requests per timespan
    /// </summary>
    public class MaxRequestsPerTimespanRateLimiter : IRateLimiter
    {
        public const long TicksPerMinute = 600000000;

        // NOTE
        // These two values must come from a config file, app settings or parameter store in the cloud
        // For this assignment I am using constants
        private const int maxRequests = 2;
        private const int requestTimeInMinutes = 1;

        private const long requestTimeInTicks = TicksPerMinute*requestTimeInMinutes;
        
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

                if (window == null || window.timestamp.Ticks + requestTimeInTicks < requestedDate.Ticks)
                    window = new RequestWindow(requestedDate, 0);


                if (window.count >= maxRequests)
                {
                    result = false;
                }
                else
                {
                    RequestWindow updatedWindow = new RequestWindow(window.timestamp, window.count + 1);
                    InMemoryStorage.requestLogByService[serviceType][userToken] = updatedWindow;
                    result = true;
                }
            }

            return result;
        }
    }
}
