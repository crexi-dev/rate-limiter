using System;

namespace RateLimiter.Core
{
    public class ClientRequestContext
    {
        public string Token { get; set; }
        public string Endpoint { get; set; }
        public string? Region { get; set; } // e.g., "US", "EU"
        public DateTime? RequestTimeUtc { get; set; }

        public ClientRequestContext(string token, string endpoint)
        {
            Token = token;
            Endpoint = endpoint;
        }

        public ClientRequestContext(string token, string endpoint, string region, DateTime requestTimeUtc)
        {
            Token = token;
            Endpoint = endpoint;
            Region = region;
            RequestTimeUtc = requestTimeUtc;
        }
    }

}
