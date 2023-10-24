using System;

namespace RateLimiter.Models
{
    public class RequestHistoryEModel
    {
        public Guid Id { get; set; }

        public string ClientId { get; set; }

        public DateTime ReqDate { get; set; }
    }
}
