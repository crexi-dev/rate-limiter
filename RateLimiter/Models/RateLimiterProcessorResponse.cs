namespace RateLimiter.Models
{
    public class RateLimiterProcessorResponse
    {
        public RateLimiterProcessorResponse(string? rateLimitProcessor)
        {
            RateLimterProcessor = rateLimitProcessor;
        }

        public string? RateLimterProcessor { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = true;
        public string? Message { get; set; } = string.Empty;
        
    }
}
