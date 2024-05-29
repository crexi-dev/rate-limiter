namespace RateLimiter.Models
{
    public record RateLimiterResult(bool IsAllowed, RateLimitError[] Errors)
    {
    }
}
