using RateLimiter.Models;

namespace RateLimiter.Services;

public interface IRateLimitValidator
{
    RateLimitValidationResult Validate(ClientData data);
}