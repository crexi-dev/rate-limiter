namespace RateLimiter.Models
{
    public class RateLimiting
    {
        public RateLimitingType Type { get; set; }
        public long Value { get; set; }

        public RateLimiting(RateLimitingType type, long value)
        {
            Type = type;
            Value = value;
        }
    }
}
