using System;

namespace RateLimiter.DataModel
{
    public class ClientRequestHistory
    {
        public int ClientId { get; set; }
        public int ResourceId { get; set; }
        public DateTime LastRequested { get; set; }
        public int Count { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
