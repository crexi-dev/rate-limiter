using System;

namespace RateLimiter.Models
{
    public class RequestHistoryEModel
    {
        public string ClientId { get; set; }

        public DateTime ReqDate { get; set; }
    }
}
