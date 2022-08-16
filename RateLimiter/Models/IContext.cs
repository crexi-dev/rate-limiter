namespace RateLimiter.Models
{
    public interface IContext
    {
        public string ClientId { get; set; }

        public string ResourceName { get; set; }

        public string Region { get; set; }
    }
}
