using System;
using System.Collections.Generic;
using System.Net;

namespace RateLimiter.Model
{
    public class ClientToken
    {
        public IPAddress Ip { get; }
        public IDictionary<string, string> Claims { get; }
        public DateTime Expiration { get; }

        public ClientToken(IPAddress ip) : this(ip, new Dictionary<string, string>()) { }
        public ClientToken(IPAddress ip, IDictionary<string, string> claims) : this(ip, claims, DateTime.UtcNow.AddDays(1)) { }
        public ClientToken(IPAddress ip, IDictionary<string, string> claims, DateTime expiration)
        {
            Ip = ip;
            Claims = claims;
            Expiration = expiration;
        }
    }
}
