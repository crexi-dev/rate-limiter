using System;
using System.Collections.Generic;
using System.Net;

namespace RateLimiter.Model
{
    public class Token
    {
        public IPAddress Ip { get; }
        public IDictionary<string, string> Claims { get; }
        public DateTime Expiration { get; }

        public Token(IPAddress ip) : this(ip, new Dictionary<string, string>()) { }
        public Token(IPAddress ip, IDictionary<string, string> claims) : this(ip, claims, DateTime.UtcNow.AddDays(1)) { }
        public Token(IPAddress ip, IDictionary<string, string> claims, DateTime expiration)
        {
            Ip = ip;
            Claims = claims;
            Expiration = expiration;
        }
    }
}
