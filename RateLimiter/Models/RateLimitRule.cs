namespace RateLimiter.Models
{
    public class RateLimitRule
    {
        public LimitStrategy Strategy { get; set; }
        public string Guard { get; set; }
        public LimitPolicy Policy { get; set; }
        public int RequestCount { get; set; }
        public int LimitInSeconds { get; set; }
        public int TimeSinceLastRequestInSeconds { get; set; }
    }
}
