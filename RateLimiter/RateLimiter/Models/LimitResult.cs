namespace RateLimiter.RateLimiter.Models
{
    public class LimitResult
    {
        public bool Limited { get; set; }
        public int CurrentLimit { get; set; }
        public int RemainingAmountOfCalls { get; set; }
        public int? RetryAfterSeconds { get; set; }
    }
}
