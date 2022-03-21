using RateLimiter.Core.Models.Enums;

namespace RateLimiter.Core.Models.Identity
{
    public class ClientRequestIdentity
    {
        public string ClientId { get; set; }
        
        public LocationEnum Location { get; set; }

        public string Path { get; set; }

        public string HttpVerb { get; set; }
    }
}