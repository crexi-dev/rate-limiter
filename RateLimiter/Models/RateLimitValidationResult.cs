namespace RateLimiter.Models;

public record RateLimitValidationResult(
    bool Result,
    int? VisitCounts = null);