namespace RateLimiter.Models
{
    public class RateLimiterStrategyResponse
    {
        public RateLimiterStrategyResponse(string? rateLimitStrategy)
        {
            RateLimitStrategy = rateLimitStrategy;
        }

        public string? RateLimitStrategy { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = true;
        public string? Message { get; set; } = string.Empty;
        
    }
}
