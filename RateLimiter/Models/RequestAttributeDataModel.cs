using RateLimiter.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models
{
    public class RequestAttributeDataModel
    {        
        public RateLimiterType RequestedType { get; set; }
        public int RequestedMaxRequests { get; set; }
        public TimeSpan RequestedTimeWindow { get; set; }
        public string[]? RequestedRegions { get; set; }
    }
}
