using System.Collections.Generic;

namespace RateLimiter.Models;

public class RateLimitSettings
{
    public static string Section => nameof(RateLimitSettings);
    public RateLimitRule CommonRule { get; set; } = null!;
    public RateLimitRule[] Regions { get; set; } = null!;
}