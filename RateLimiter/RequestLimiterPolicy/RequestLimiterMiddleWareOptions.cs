namespace RateLimiter.RequestLimiterPolicy
{
    public record class RequestLimiterMiddleWareOptions
    {
        public uint Amount { get; set; }
        public uint TimeSpan { get; set; }
    }
}
