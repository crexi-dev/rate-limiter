using System.Collections.Generic;

namespace RateLimiter.Contracts
{
    public interface IRateLimiterSettings
    {
        IEnumerable<IRequestLimitRule> Rules { get; set; }
    }
}
