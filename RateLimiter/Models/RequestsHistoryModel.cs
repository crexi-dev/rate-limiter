using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models
{
    public class RequestsHistoryModel
    {
        public DateTime DateTime { get; set; }
        
        public string RegionName { get; set; }
    }
}
