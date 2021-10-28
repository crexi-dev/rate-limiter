using RateLimiter.Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Model.Request
{
    public class ApiRequest
    {
        public DateTime DateRequested { get; set; } = DateTime.UtcNow;

        public ResourceEnum ResourceName { get; set; }
    }
}
