using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models
{
    public class ClientRequest
    {
        // Endpoint
        public string Resource { get; set; }

        // ClientId from httpContext
        public string ClientId { get; set; }

        // Time When request was made
        public DateTime RequestTime { get; set; }

        // HttpVerb
        public string Method { get; set; }
    }
}
