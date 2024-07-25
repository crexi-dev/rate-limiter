using RateLimiter.Enums;
using System;
using System.Collections.Generic;

namespace RateLimiter
{
    /// <summary>
    /// This Class should be replaced by a Database or a Cache like Redis
    /// </summary>
    public record RequestWindow(DateTime timestamp, int count);
    public class InMemoryStorage
    {
        public static Dictionary<ServiceType, Dictionary<string, RequestWindow>> requestLogByService = new Dictionary<ServiceType, Dictionary<string, RequestWindow>>();
    }
}
