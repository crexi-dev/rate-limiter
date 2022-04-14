using System;

namespace RateLimiter.Models
{
    public class Request
    {
        public Request(string key, string ip, string endpoint)
        {
            Key = key;
            IpAddress = ip;
            Destination = endpoint;
        }

        public string Key { get; set; }
        public string IpAddress { get; set; }
        public string Destination { get; set; }
        public DateTime Timestamp { get; set; }
        public string Payload { get; set; }
    }
}
