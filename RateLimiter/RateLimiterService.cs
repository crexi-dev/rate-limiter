using System.Collections.Generic;

namespace RateLimiter
{
    public static class RateLimiterService
    {
        public static Dictionary<string, SlidingWindow> RequestsThrottle = new Dictionary<string, SlidingWindow>();

        public static bool AllowRequest(string id, ClientConfig settings) 
        {
            if (!RequestsThrottle.ContainsKey(id)) 
            {
                RequestsThrottle.Add(id, new SlidingWindow(settings.MaxRequestPerTime?.Milliseconds, settings.MaxRequestPerTime?.Max, settings.PassedSinceLastCall?.Milliseconds));
            }
            return RequestsThrottle[id].AllowRequest();
        }
    }
}
