using RateLimiter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models
{
    public class RequestModel
    {
        public string? UserToken { get; set; }
        public ELocation ClientLocation { get; set; }

        public DateTime timeStamp = DateTime.Now;
    }
}
