using System;

namespace RateLimiter
{
    public class ApiRequest
    {
        public DateTime DateRequested { get; set; } = DateTime.UtcNow;

        public string ResourceName { get; set; }
    }
}
