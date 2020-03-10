using System.Collections.Generic;

namespace RateLimiter.Rules
{
    public interface IRateLimitRule
    {
        bool Validate(string token, string resourceName, List<ApiRequest> tokenRequestLog);
    }
}
