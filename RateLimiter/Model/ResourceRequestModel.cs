using System;

namespace RateLimiter.Model
{
    public class ResourceRequestModel
    {
        public string ResourceName { get; set; }

        public string AccessToken { get; set; }

        public DateTime UtcDateRequest { get; set; }
    }
}
