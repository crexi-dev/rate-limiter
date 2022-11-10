using RateLimiter.Enums;
using System;

namespace RateLimiter.Models.Request
{
    public class ClientRequest
    {
        public Guid ClientId { get; set; } = Guid.NewGuid();

        public string? Endpoint { get; set; }

        public DateTime RequestDate { get; set; }

        public Location Location { get; set; }
    }
}