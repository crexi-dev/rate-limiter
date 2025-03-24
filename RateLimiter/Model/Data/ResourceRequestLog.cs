using System;

namespace RateLimiter.Model.Data
{
    public class ResourceRequestLog
    {
        public int ResourceId { get; set; }
        public int ResourceConfigId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;        
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
