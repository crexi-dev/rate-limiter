using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Abstractions;

public interface IRateLimitService
{
    Task<IEnumerable<IRateLimit>> GetRateLimitsAsync(string apiKey);
}