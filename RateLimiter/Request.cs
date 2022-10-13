using System;

namespace RateLimiter
{
    public class Request
    {
        public Request()
        {
            DateTime = DateTime.UtcNow;
        }

        public Guid ClientId { get; set; }

        public ClientLocation ClientLocation { get; set; }

        public DateTime DateTime { get; set; }
    }
}
