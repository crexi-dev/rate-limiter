using System;

namespace RateLimiter.Domain
{
    public sealed class UserRequest
    {
        public string AccessToken { get; set; }

        public DateTime Date { get; set; }

        public string ResourceName { get; set; }    
    }
}
