namespace RateLimiter.RateLimit
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RateLimitAttribute : Attribute
    {
        public int TimeWindowInSeconds { get; set; }
        public int MaxRequests { get; set; }
    }
}
