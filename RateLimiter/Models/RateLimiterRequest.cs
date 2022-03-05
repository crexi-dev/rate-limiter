using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models
{
    public class RateLimiterRequest
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string GroupId { get; set; } = Guid.NewGuid().ToString();
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public RateLimiterRequestCountry? Country { get; set; }
        public DateTime RequestDate { get; set; }
        public RateLimiterResponse Response { get; set; }
    }
}
