namespace RateLimiter.Models
{
    public sealed class RateLimiterContext : IContext
    {
        public string ClientId { get; set; }

        public string ResourceName { get; set; }

        public string Region { get; set; }
    }
}
