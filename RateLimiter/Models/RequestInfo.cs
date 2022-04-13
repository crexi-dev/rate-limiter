using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RateLimiter.Models
{
    /// <summary>
    /// This class stores information about the request, including the token and geolocation metadata
    /// </summary>
    public class RequestInfo
    {
        public Token Token { get; }
        public Location Location { get; }

        public DateTime DateTime { get; }

        public RequestInfo(Token token, Location location) : this(token, location, DateTime.UtcNow) { }

        public RequestInfo(Token token, Location location, DateTime dateTime)
        {
            this.Token = token;
            this.Location = location;
            this.DateTime = dateTime;
        }
    }
}
