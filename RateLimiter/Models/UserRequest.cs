using RateLimiter.Enums;
using System;

namespace RateLimiter.Models
{
    public class UserRequest
    {
        public string userToken {  get; set; } = string.Empty;
        public ServiceType requestedServiceType { get; set; }
        public DateTime requestedDate { get; set; }
    }
}
