using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models
{
    /// <summary>
    /// A simple class to represent the location from where the request came
    /// </summary>
    public class Location
    {
        public string IpAddress { get; }

        public Region Region { get; }

        public Location(string ipAddress, Region region)
        {
            this.IpAddress = ipAddress;
            this.Region = region;
        }
    }
}
