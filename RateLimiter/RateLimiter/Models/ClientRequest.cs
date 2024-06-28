using System;

namespace RateLimiter.RateLimiter.Models
{
    public class ClientRequest
    {
        public DateTime LastHitAt { get; set; }
        public int AmountOfHits { get; set; }

        public static ClientRequest Empty => new ClientRequest
        {
            AmountOfHits = 0,
            LastHitAt = DateTime.MinValue.ToUniversalTime(),
        };
    }
}
