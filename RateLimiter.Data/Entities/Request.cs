using RateLimiter.Data.Enums;
using System;
using System.Net;

namespace RateLimiter.Data
{
    public class Request
    {
        public string AccessToken { get; set;  }

        public DateTime RequestAtDateTime { get; } = DateTime.UtcNow;

        public string Resource { get; set; }

        public string Ip { get; set; }


        public string GetIdentifier() => AccessToken;

        public RegionTypeEnum GetRegion()
        {
            var isIpUsa = Ip?.ToString().StartsWith("192.") == true;

            return isIpUsa ? RegionTypeEnum.USA : RegionTypeEnum.EU;
        }
    }
}
