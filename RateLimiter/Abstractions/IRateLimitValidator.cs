using System.Threading.Tasks;

namespace RateLimiter.Abstractions;

public interface IRateLimitValidator
{
    Task ValidateAsync(string apiKey);
}