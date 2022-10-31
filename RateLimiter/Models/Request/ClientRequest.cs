using RateLimiter.Enums;
using System;

namespace RateLimiter.Models.Request
{
    public class ClientRequest
    {
        public string? ClientId { get; set; } = Guid.NewGuid().ToString();

        public ClientLocation Location { get; set; }
    }
}
