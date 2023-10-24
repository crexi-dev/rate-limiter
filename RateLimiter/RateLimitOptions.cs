using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimitOptions
    {
        public static string SectionName { get; internal set; } = "RateLimit";

        public string AccessTokenName { get; set; }

        public string ClientIdName { get; set; }

        public string TokenRegionName { get; set; }

        public bool ProcessRequestIfNotHandled { get; set; }

        public ClientOptions[] ClientOptions { get; set; }
    }
}
