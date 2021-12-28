using System;

namespace RateLimiter.Entities
{
    public class ClientRequestModel
    {
        public string Token { get; set; }
        public int RequestId { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
