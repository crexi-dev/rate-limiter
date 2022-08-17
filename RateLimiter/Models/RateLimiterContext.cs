using System.Collections.Generic;

namespace RateLimiter.Models
{
    public sealed class RateLimiterContext : IContext
    {
        public string ClientId { get; set; }

        public string ResourceName { get; set; }

        public string Region { get; set; }

        public bool? IsClientAuthenticated { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public bool? HasSubscription { get; set; }
    }
}
